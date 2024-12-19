:- use_module(library(http/thread_httpd)).
:- use_module(library(http/http_dispatch)).
:- use_module(library(http/http_json)).

:- dynamic availability/3.
:- dynamic agenda_staff/3.
:- dynamic agenda_staff1/3.
:- dynamic agenda_operation_room/3.
:- dynamic agenda_operation_room1/3.
:- dynamic better_sol/5.

:- dynamic timetable/3.
:- dynamic surgery/4.
:- dynamic surgery_id/2.
:- dynamic staff/3.
:- dynamic assignment_surgery/2.
:- dynamic agenda_operation_room/3.

:- consult('algav.pl').

%
:- http_handler('/obtain_better', obtain_better, [method(post)]).

% Inicia o servidor na porta especificada
start_server(Port) :-        
    http_server(http_dispatch, [port(Port)]).

% Para o servidor
stop_server :-
    server_port(Port),
    http_stop_server(Port, []).

% Handler para a rota "/obtain_better"
obtain_better(Request) :-
    retractall(agenda_staff(_, _, _)),
    retractall(timetable(_, _, _)),
    retractall(surgery(_, _, _, _)),
    retractall(surgery_id(_, _)),
    retractall(staff(_, _, _)),
    retractall(assignment_surgery(_, _)),
    retractall(agenda_operation_room(_, _, _)),

    % Read the JSON request body as a dictionary
    http_read_json_dict(Request, Body),

    % Extract fields from the JSON dictionary
    get_dict(facts, Body, Facts),
    get_dict(room, Body, Room),
    get_dict(day, Body, Day),

    % Process the facts
    process_facts(Facts),

    %obtain_better_sol(Room,Day,AgOpRoomBetter,LAgDoctorsBetter,TFinOp):-
    obtain_better_sol(Room, Day, AgendaRoom, AgendaDoctors, FinOp),

    convert_tuples_to_lists(AgendaRoom, ProcessedAgendaRoom),
    convert_agenda_doctors(AgendaDoctors, ProcessedAgendaDoctors),

    % Return a structured JSON response
    reply_json_dict(_{
        result: 'success1',
        room: Room,
        day: Day,
        agenda_room: ProcessedAgendaRoom,
        agenda_doctors: ProcessedAgendaDoctors,
        finOp: FinOp
    }).


convert_tuples_to_lists([], []).
convert_tuples_to_lists([(A, B, C) | Rest], [[A, B, C] | ConvertedRest]) :-
    convert_tuples_to_lists(Rest, ConvertedRest).

convert_agenda_doctors([], []).
convert_agenda_doctors([(ID, Slots) | Rest], [_{id: ID, slots: ConvertedSlots} | ConvertedRest]) :-
    convert_tuples_to_lists(Slots, ConvertedSlots),
    convert_agenda_doctors(Rest, ConvertedRest).

process_facts([]).
process_facts([Fact | Rest]) :-
    assert_fact(Fact),
    process_facts(Rest).

assert_fact(Fact) :-
    atom_string(AtomFact, Fact),         
    term_to_atom(TermFact, AtomFact),    
    asserta(TermFact).                   
             



% Exemplo de chamada:
% process_facts(["agenda_staff(T202400001,20251010,[])", "agenda_staff(S202400001,20251010,[])"]).
% atom_string(AtomFact, 'availability(1, 1, 1)'),
% term_to_atom(TermFact, AtomFact),
% assertz(availability(1, 1, 1)).
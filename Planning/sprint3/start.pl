:- dynamic availability/3.
:- dynamic agenda_staff/3.
:- dynamic agenda_staff1/3.
:- dynamic agenda_operation_room/3.
:- dynamic agenda_operation_room1/3.
:- dynamic better_sol/5.

:- dynamic room_surgery/2.

:- consult('schedule.pl').
:- consult('schedule_genetic_algorithm.pl').
:- consult('base_schedule.pl').

% parameters initialization
initialize:-
    write('Number of new generations: '),read(NG), 			
    (retract(generations(_));true), asserta(generations(NG)),
	write('Probability of crossover (%):'), read(P1),
	PC is P1/100, 
	(retract(prob_crossover(_));true), 	asserta(prob_crossover(PC)),
	write('Probability of mutation (%):'), read(P2), 
	PM is P2/100, 
	(retract(prob_mutation(_));true), asserta(prob_mutation(PM)),

    write('Achieve fit to stop: '),read(FIT),
	(retract(min_fitness(_));true), asserta(min_fitness(FIT)),
    write('Time limit (seconds): '),read(TL),
    (retract(time_limit(_));true), asserta(time_limit(TL)),
    write('Stabilize generations: '),read(SG),
    (retract(stabilize_generations(_));true), asserta(stabilize_generations(SG)),
    
    
    retractall(agenda_staff1(_,_,_)),
    retractall(agenda_operation_room1(_,_,_)),
    retractall(availability(_,_,_)),
    findall(_,(agenda_staff(D,Day,Agenda),assertz(agenda_staff1(D,Day,Agenda))),_),
    findall(_,(agenda_operation_room(Room,Day,Agenda),assertz(agenda_operation_room1(Room,Day,Agenda))),_),
    findall(_,(agenda_staff1(D,Day,L),free_agenda0(L,LFA),adapt_timetable(D,Day,LFA,LFA2),assertz(availability(D,Day,LFA2))),_)
    .

schedule_genetic(Day):-
    initialize,

    retractall(day(_)), asserta(day(Day)),
    retractall(room_surgery(_, _)),

    findall(Room, agenda_operation_room(Room, Day, _), LRoom),

    % Sort surgeries by time
    findall(OpCode, surgery_id(OpCode, _), LOpCode),
    sort_surgeries_by_time(LOpCode, SortedLOpCode),

    (check_room_capacity(LRoom, SortedLOpCode, Day, FinalRoom) -> 
    	write('Population size: '),read(PS),
	    (retract(population(_));true), asserta(population(PS)),

        % if the surgeries fit all in one room
        retractall(surgeries(_)),
        length(SortedLOpCode, SurgeryCount),
        asserta(surgeries(SurgeryCount)),
        retractall(room(_)),asserta(room(FinalRoom)),
        assign_surgeries_to_room(SortedLOpCode, FinalRoom),

        generate(FinalGeneration), % call genetic algorithm outputing FinalGeneration
        FinalGeneration = [BestInd*_ | _], % get best individual
        availability_all_surgeries(BestInd, FinalRoom, Day) % schedule surgeries
    ; 
        % distribute surgeries by room
        distribute_surgeries_by_room(LRoom, SortedLOpCode, Day),
        iterate_through_rooms(LRoom, Day)     
    ).

iterate_through_rooms([], _) :- !.
iterate_through_rooms([Room|LRoom], Day) :- 
    retractall(room(_)),asserta(room(Room)),
    retractall(surgeries(_)),
    findall(OpCode, room_surgery(Room, OpCode), LOpCode),
    length(LOpCode, SurgeryCount),
    asserta(surgeries(SurgeryCount)),
    	
    write('Population size N:'), write(SurgeryCount), write(': '), read(PS),
	(retract(population(_));true), asserta(population(PS)), 

    generate(FinalGeneration), % call genetic algorithm outputing FinalGeneration

    FinalGeneration = [BestInd*_ | _], % get best individual
    
    availability_all_surgeries(BestInd, Room, Day), % schedule surgeries
    iterate_through_rooms(LRoom, Day).

assign_surgeries_to_room([], _) :- !.
assign_surgeries_to_room(_, []) :- !.
assign_surgeries_to_room([OpCode|LOpCode], Room) :-
    assign_surgery_to_room(OpCode, Room),
    assign_surgeries_to_room(LOpCode, Room).


distribute_surgeries_by_room([], _, _) :- !. 
distribute_surgeries_by_room(_, [], _) :- !.
distribute_surgeries_by_room(LRoom, SortedLOpCode, Day) :-
    distribute_surgeries_by_room1(LRoom, SortedLOpCode, Day, 0).

distribute_surgeries_by_room1(_, [], _, _) :- !. 
distribute_surgeries_by_room1(LRoom, [OpCode|LOpCode], Day, Index) :-
    length(LRoom, RoomCount),
    RoomIndex is Index mod RoomCount,
    nth0(RoomIndex, LRoom, AssignedRoom), 
    check_room_fit_surgery(AssignedRoom, OpCode, Day) -> 
        % if true
        assign_surgery_to_room(OpCode, AssignedRoom),
        NextIndex is Index + 1,
        distribute_surgeries_by_room1(LRoom, LOpCode, Day, NextIndex)
    ; % else
    NextIndex is Index + 1,
    distribute_surgeries_by_room1(LRoom, LOpCode, Day, NextIndex).

check_room_fit_surgery(Room, OpCode, Day):-
    surgery_id(OpCode, Type),
    surgery(Type, TAnesthesia, TSurgery, TCleaning),
    agenda_operation_room(Room, Day, Agenda),
    free_agenda0(Agenda, LFAgRoom),
    get_room_free_time(LFAgRoom, TotalRoomTime),
    TotalTime is TAnesthesia + TSurgery + TCleaning,
    TotalTime =< TotalRoomTime.

assign_surgery_to_room(OpCode, Room) :-
    assertz(room_surgery(Room, OpCode)).

check_room_capacity([], _, _, _) :- 
    !,
    false.
check_room_capacity([Room|LRoom], SortedLOpCode, Day, FinalRoom) :-
    get_total_surgeries_time(SortedLOpCode, TotalTime),
    
    agenda_operation_room(Room, Day, Agenda),
    free_agenda0(Agenda, LFAgRoom),
    get_room_free_time(LFAgRoom, TotalRoomTime),

    ( TotalTime =< TotalRoomTime ->
        FinalRoom = Room
    ; 
        check_room_capacity(LRoom, SortedLOpCode, Day, FinalRoom)
    ).



get_room_free_time([], 0).
get_room_free_time([(Tin, Tfin)|LFAgRoom], TotalRoomTime) :-
    get_room_free_time(LFAgRoom, TotalRoomTime1),
    TotalRoomTime is TotalRoomTime1 + Tfin - Tin.


get_total_surgeries_time([], 0).
get_total_surgeries_time([OpCode|LOpCode], TotalTime) :-
    surgery_id(OpCode, Type),
    surgery(Type, TAnesthesia, TSurgery, TCleaning),
    get_total_surgeries_time(LOpCode, TotalTime1),
    TotalTime is TotalTime1 + TAnesthesia + TSurgery + TCleaning.


compare_surgery_time(<, OpCode1, OpCode2) :-
    surgery_id(OpCode1, Type),
    surgery(Type, TAnesthesia, TSurgery, TCleaning),
    surgery_id(OpCode2, Type1),
    surgery(Type1, TAnesthesia1, TSurgery1, TCleaning1),
    Time1 is TAnesthesia + TSurgery + TCleaning,
    Time2 is TAnesthesia1 + TSurgery1 + TCleaning1,
    Time1 =< Time2.

compare_surgery_time(>, OpCode1, OpCode2) :-
    surgery_id(OpCode1, Type),
    surgery(Type, TAnesthesia, TSurgery, TCleaning),
    surgery_id(OpCode2, Type1),
    surgery(Type1, TAnesthesia1, TSurgery1, TCleaning1),
    Time1 is TAnesthesia + TSurgery + TCleaning,
    Time2 is TAnesthesia1 + TSurgery1 + TCleaning1,
    Time1 > Time2.

sort_surgeries_by_time(Unsorted, Sorted) :-
    predsort(compare_surgery_time, Unsorted, Sorted).

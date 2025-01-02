:- dynamic availability/3.
:- dynamic agenda_staff/3.
:- dynamic agenda_staff1/3.
:- dynamic agenda_operation_room/3.
:- dynamic agenda_operation_room1/3.
:- dynamic better_sol/5.

availability_all_surgeries([],_,_).
availability_all_surgeries([OpCode|LOpCode],Room,Day):-
    surgery_id(OpCode,OpType),
    surgery(OpType,TAnesthesia,TSurgery,TCleaning),

    findall(ID, (assignment_surgery(OpCode,ID), (staff(ID,doctor,orthopaedist) ; staff(ID,nurse,instrumenting) ; staff(ID,nurse,circulating))), LDoctors),
    findall(ID, (assignment_surgery(OpCode,ID), staff(ID,_,anaesthetist)), LAnesthetists),
    findall(ID, (assignment_surgery(OpCode,ID), staff(ID,assistant,_)), LAssistants),

    intersect_all_agendas(LDoctors,Day,LDoctorsAgendas1),
    remove_unf_intervals(TSurgery,LDoctorsAgendas1,LDoctorsAgendas),
    
    Tianas is TAnesthesia+TSurgery,
    intersect_all_agendas(LAnesthetists,Day,LAnesthetistsAgendas1),
    remove_unf_intervals(Tianas,LAnesthetistsAgendas1,LAnesthetistsAgendas),

    intersect_all_agendas(LAssistants,Day,LAssistantsAgendas1),
    remove_unf_intervals(TCleaning,LAssistantsAgendas1,LAssistantsAgendas),
    reverse(LAssistantsAgendas, ListASSR),
    availability_operation(OpCode,Room,Day,LAnesthetistsAgendas,ListASSR,LPossibilities),
    TTotal is TAnesthesia+TSurgery+TCleaning,
    Count is 0,
    schedule_first_interval(Count, TTotal,LPossibilities,(TinS,TfinS), NewLPossibilities, NewCount),
        
    loopToScheduleInCorrectTime(NewCount, NewLPossibilities, OpCode, LOpCode, Room, Day, TTotal, TAnesthesia, TCleaning, LDoctorsAgendas, LAnesthetistsAgendas, LAssistantsAgendas, Doctor).
 

loopToScheduleInCorrectTime(Count1, LPossibilities, OpCode, LOpCode, Room, Day, TTotal, TAnesthesia, TCleaning, LDoctorsAgendas, LAnesthetistsAgendas, LAssistantsAgendas, Doctor):-
    schedule_first_interval(Count1, TTotal,LPossibilities,(TinS,TfinS), NewLPossibilities, Count),
    TfiAn is TfinS-TCleaning,
    TiDoc is TinS+TAnesthesia,
    intersect_2_agendas(LDoctorsAgendas, [(TiDoc,TfiAn)], LDoctorsAgendas2),
    intersect_2_agendas(LAnesthetistsAgendas, [(TinS,TfiAn)], LAnesthetistsAgendas2),
    intersect_2_agendas(LAssistantsAgendas, [(TfiAn,TfinS)], LAssistantsAgendas2),
 
    %findall(Doctor,assignment_surgery(OpCode,Doctor),LStaff),
    (   
        LDoctorsAgendas2 == [(TiDoc,TfiAn)],
        LAnesthetistsAgendas2 == [(TinS,TfiAn)],
        LAssistantsAgendas2 == [(TfiAn,TfinS)]
    ->  !,
        findall(Doctor, assignment_surgery(OpCode, Doctor), LStaff),
        retract(agenda_operation_room1(Room, Day, Agenda)),
        insert_agenda((TinS, TfinS, OpCode), Agenda, Agenda1),
        assertz(agenda_operation_room1(Room, Day, Agenda1)),
        insert_agenda_doctors((TinS, TfinS, OpCode), Day, LStaff),
        availability_all_surgeries(LOpCode, Room, Day)
    ;   
        NewCount is Count + 1,
        loopToScheduleInCorrectTime(NewCount, NewLPossibilities, OpCode, LOpCode, Room, Day, TTotal, TAnesthesia, TCleaning, LDoctorsAgendas, LAnesthetistsAgendas, LAssistantsAgendas, Doctor)
    ).
 
availability_operation(OpCode,Room,Day, [(TiAnas,_)|_], [(_,TfAss)|_], LPossibilities):-
    surgery_id(OpCode,OpType),
    surgery(OpType,TAnesthesia,TSurgery,TCleaning),
    TTotal is TAnesthesia+TSurgery+TCleaning,
    agenda_operation_room1(Room,Day,LAgenda),
    free_agenda0(LAgenda,LFAgRoom),
    remove_unf_intervals(TTotal,LFAgRoom,LPossibilities1),
    intersect_2_agendas(LPossibilities1,[(TiAnas,TfAss)],LPossibilities).


obtain_better_sol(Room,Day,AgOpRoomBetter,LAgDoctorsBetter,TFinOp):-
		get_time(Ti),
		(obtain_better_sol1(Room,Day);true),
		retract(better_sol(Day,Room,AgOpRoomBetter,LAgDoctorsBetter,TFinOp)),
            nl,write('Final Result: AgOpRoomBetter='),write(AgOpRoomBetter),nl,nl,
            write('LAgDoctorsBetter='),write(LAgDoctorsBetter),nl,nl,
            write('TFinOp='),write(TFinOp),nl,
		get_time(Tf),
		T is Tf-Ti,
		write('Tempo de geracao da solucao:'),write(T),nl.

obtain_better_sol1(Room,Day):-
    asserta(better_sol(Day,Room,_,_,1441)),
    findall(OpCode,surgery_id(OpCode,_),LOC),!,
    permutation(LOC,LOpCode),
    retractall(agenda_staff1(_,_,_)),
    retractall(agenda_operation_room1(_,_,_)),
    retractall(availability(_,_,_)),
    findall(_,(agenda_staff(D,Day,Agenda),assertz(agenda_staff1(D,Day,Agenda))),_),
    agenda_operation_room(Room,Day,Agenda),assert(agenda_operation_room1(Room,Day,Agenda)),
    findall(_,(agenda_staff1(D,Day,L),free_agenda0(L,LFA),adapt_timetable(D,Day,LFA,LFA2),assertz(availability(D,Day,LFA2))),_),
    availability_all_surgeries(LOpCode,Room,Day),
    agenda_operation_room1(Room,Day,AgendaR),
		update_better_sol(Day,Room,AgendaR,LOpCode),
		fail.

update_better_sol(Day,Room,Agenda,LOpCode):-
                better_sol(Day,Room,_,_,FinTime),
                reverse(Agenda,AgendaR),
                evaluate_final_time(AgendaR,LOpCode,FinTime1),
             write('Analysing for LOpCode='),write(LOpCode),nl,
             write('now: FinTime1='),write(FinTime1),write(', now: FinTime='),write(FinTime),write(' Agenda='),write(Agenda),nl,
        FinTime1<FinTime,
                write('best solution updated'),nl,
                retract(better_sol(_,_,_,_,_)),
                findall(Doctor,assignment_surgery(_,Doctor),LDoctors1),
                remove_equals(LDoctors1,LDoctors),
                list_doctors_agenda(Day,LDoctors,LDAgendas),
                asserta(better_sol(Day,Room,Agenda,LDAgendas,FinTime1)).


evaluate_final_time([],_,1441).
evaluate_final_time([(_,Tfin,OpCode)|_],LOpCode,Tfin):-member(OpCode,LOpCode),!.
evaluate_final_time([_|AgR],LOpCode,Tfin):-evaluate_final_time(AgR,LOpCode,Tfin).

list_doctors_agenda(_,[],[]).
list_doctors_agenda(Day,[D|LD],[(D,AgD)|LAgD]):-agenda_staff1(D,Day,AgD),list_doctors_agenda(Day,LD,LAgD).

remove_equals([],[]).
remove_equals([X|L],L1):-member(X,L),!,remove_equals(L,L1).
remove_equals([X|L],[X|L1]):-remove_equals(L,L1).

free_agenda0([],[(0,1440)]).
free_agenda0([(0,Tfin,_)|LT],LT1):-!,free_agenda1([(0,Tfin,_)|LT],LT1).
free_agenda0([(Tin,Tfin,_)|LT],[(0,T1)|LT1]):- T1 is Tin-1,
    free_agenda1([(Tin,Tfin,_)|LT],LT1).

free_agenda1([(_,Tfin,_)],[(T1,1440)]):-Tfin\==1440,!,T1 is Tfin+1.
free_agenda1([(_,_,_)],[]).
free_agenda1([(_,T,_),(T1,Tfin2,_)|LT],LT1):-Tx is T+1,T1==Tx,!,
    free_agenda1([(T1,Tfin2,_)|LT],LT1).
free_agenda1([(_,Tfin1,_),(Tin2,Tfin2,_)|LT],[(T1,T2)|LT1]):-T1 is Tfin1+1,T2 is Tin2-1,
    free_agenda1([(Tin2,Tfin2,_)|LT],LT1).


adapt_timetable(D,Date,LFA,LFA2):-timetable(D,Date,(InTime,FinTime)),treatin(InTime,LFA,LFA1),treatfin(FinTime,LFA1,LFA2).

treatin(InTime,[(In,Fin)|LFA],[(In,Fin)|LFA]):-InTime=<In,!.
treatin(InTime,[(_,Fin)|LFA],LFA1):-InTime>Fin,!,treatin(InTime,LFA,LFA1).
treatin(InTime,[(_,Fin)|LFA],[(InTime,Fin)|LFA]).
treatin(_,[],[]).

treatfin(FinTime,[(In,Fin)|LFA],[(In,Fin)|LFA1]):-FinTime>=Fin,!,treatfin(FinTime,LFA,LFA1).
treatfin(FinTime,[(In,_)|_],[]):-FinTime=<In,!.
treatfin(FinTime,[(In,_)|_],[(In,FinTime)]).
treatfin(_,[],[]).


intersect_all_agendas([Name],Date,LA):-!,availability(Name,Date,LA).
intersect_all_agendas([Name|LNames],Date,LI):-
    availability(Name,Date,LA),
    intersect_all_agendas(LNames,Date,LI1),
    intersect_2_agendas(LA,LI1,LI).

intersect_2_agendas([],_,[]).
intersect_2_agendas([D|LD],LA,LIT):-	intersect_availability(D,LA,LI,LA1),
					intersect_2_agendas(LD,LA1,LID),
					append(LI,LID,LIT).

intersect_availability((_,_),[],[],[]).

intersect_availability((_,Fim),[(Ini1,Fim1)|LD],[],[(Ini1,Fim1)|LD]):-
		Fim<Ini1,!.

intersect_availability((Ini,Fim),[(_,Fim1)|LD],LI,LA):-
		Ini>Fim1,!,
		intersect_availability((Ini,Fim),LD,LI,LA).

intersect_availability((Ini,Fim),[(Ini1,Fim1)|LD],[(Imax,Fmin)],[(Fim,Fim1)|LD]):-
		Fim1>Fim,!,
		min_max(Ini,Ini1,_,Imax),
		min_max(Fim,Fim1,Fmin,_).

intersect_availability((Ini,Fim),[(Ini1,Fim1)|LD],[(Imax,Fmin)|LI],LA):-
		Fim>=Fim1,!,
		min_max(Ini,Ini1,_,Imax),
		min_max(Fim,Fim1,Fmin,_),
		intersect_availability((Fim1,Fim),LD,LI,LA).


min_max(I,I1,I,I1):- I<I1,!.
min_max(I,I1,I1,I).

% This predicate is used to remove intervals that are smaller than the time needed
remove_unf_intervals(_,[],[]).
remove_unf_intervals(TSurgery,[(Tin,Tfin)|LA],[(Tin,Tfin)|LA1]):-DT is Tfin-Tin+1,TSurgery=<DT,!,
    remove_unf_intervals(TSurgery,LA,LA1).
remove_unf_intervals(TSurgery,[_|LA],LA1):- remove_unf_intervals(TSurgery,LA,LA1).


schedule_first_interval(Count, TSurgery, [(Tin, End) | Rest], (TinS, TfinS), NewLPossibilities, NewCount) :-
    TfinS is (Tin + Count) + TSurgery - 1, 
    TinS is Tin + Count,
    (   TfinS >= End
    ->  !, NewLPossibilities = Rest, NewCount is 0           
    ;   
        NewLPossibilities = [(Tin, End) | Rest], NewCount is Count
    ).



insert_agenda((TinS,TfinS,OpCode),[],[(TinS,TfinS,OpCode)]).
insert_agenda((TinS,TfinS,OpCode),[(Tin,Tfin,OpCode1)|LA],[(TinS,TfinS,OpCode),(Tin,Tfin,OpCode1)|LA]):-TfinS<Tin,!.
insert_agenda((TinS,TfinS,OpCode),[(Tin,Tfin,OpCode1)|LA],[(Tin,Tfin,OpCode1)|LA1]):-insert_agenda((TinS,TfinS,OpCode),LA,LA1).

insert_agenda_doctors(_,_,[]).
insert_agenda_doctors((TinS,TfinS,OpCode),Day,[Doctor|LDoctors]):-
    adjust_time((TinS, TfinS, Doctor, OpCode), (Tinicial, Tfinal)),
    retract(agenda_staff1(Doctor,Day,Agenda)),
    insert_agenda((Tinicial,Tfinal,OpCode),Agenda,Agenda1),
    assert(agenda_staff1(Doctor,Day,Agenda1)),
    insert_agenda_doctors((TinS,TfinS,OpCode),Day,LDoctors).

%  se for cirugia Tinicial = TinS + TAnesthesia e Tfinal = TfinS - TCleaning
%  se for anestesia Tinicial = TinS e Tfinal = TfinS - TCleaning
%  se for limpeza Tinicial = TfinS - TCleaning e Tfinal = TfinS
adjust_time((TinS, TfinS, Id, OpCode), (Tinicial, Tfinal)) :-
    staff(Id, Role, Type),
    surgery_id(OpCode, OpType),
    surgery(OpType, TAnesthesia, _, TCleaning),
    (   Role = doctor, Type = orthopaedist
    ->  
        Tinicial is TinS + TAnesthesia,
        Tfinal is TfinS - TCleaning

    ;   Role = nurse, Type = anaesthetist
    ->  !, 
        Tinicial is TinS,
        Tfinal is TfinS - TCleaning

    ;   Role = nurse
    ->  
        Tinicial is TinS + TAnesthesia,
        Tfinal is TfinS - TCleaning

    ;   Role = doctor, Type = anaesthetist
    ->  
        Tinicial is TinS,
        Tfinal is TfinS - TCleaning

    ;   Role = assistant
    ->  
        Tinicial is TfinS-TCleaning,
        Tfinal is TfinS
    ).
        

:-dynamic generations/1.
:-dynamic population/1.
:-dynamic prob_crossover/1.
:-dynamic prob_mutation/1.

:-dynamic min_fitness/1.
:-dynamic time_limit/1.
:-dynamic stabilize_generations/1.
:-dynamic start_time/1.
:-dynamic count_stable_generations/1.

:-dynamic day/1.
:-dynamic room/1.

:-consult('schedule.pl').
:-consult('base_schedule.pl').

% parameters initialization
initialize:-
    write('Number of new generations: '),read(NG), 			
    (retract(generations(_));true), asserta(generations(NG)),
	write('Population size: '),read(PS),
	(retract(population(_));true), asserta(population(PS)),
	write('Probability of crossover (%):'), read(P1),
	PC is P1/100, 
	(retract(prob_crossover(_));true), 	asserta(prob_crossover(PC)),
	write('Probability of mutation (%):'), read(P2), 
	PM is P2/100, 
	(retract(prob_mutation(_));true), asserta(prob_mutation(PM)),

    write('Day:'), read(Day),
    retractall(day(_)), asserta(day(Day)),

    write('Room:'), read(Room),
    retractall(room(_)), asserta(room(Room)),

    write('Achieve fit to stop: '),read(FIT),
	(retract(min_fitness(_));true), asserta(min_fitness(FIT)),
    write('Time limit (seconds): '),read(TL),
    (retract(time_limit(_));true), asserta(time_limit(TL)),
    write('Stabilize generations: '),read(SG),
    (retract(stabilize_generations(_));true), asserta(stabilize_generations(SG)).

generate:-
    initialize,
    generate_population(Pop),
    write('Pop='),write(Pop),nl,

    day(Day),
    room(Room),

    retractall(agenda_staff1(_,_,_)),
    retractall(agenda_operation_room1(_,_,_)),
    retractall(availability(_,_,_)),
    findall(_,(agenda_staff(D,Day,Agenda),assertz(agenda_staff1(D,Day,Agenda))),_),
    agenda_operation_room(Room,Day,Agenda),assert(agenda_operation_room1(Room,Day,Agenda)),
    findall(_,(agenda_staff1(D,Day,L),free_agenda0(L,LFA),adapt_timetable(D,Day,LFA,LFA2),assertz(availability(D,Day,LFA2))),_),
    evaluate_population(Pop,PopValue),
        
    write('PopValue='),write(PopValue),nl,
    order_population(PopValue,PopOrd),
    generations(NG),

    get_time(Now),
    retractall(start_time(_)),
    asserta(start_time(Now)),

    retractall(count_stable_generations(_)),   
    asserta(count_stable_generations(0)),
    generate_generation(0,NG,PopOrd).

generate_population(Pop):-
    population(PopSize),
    surgeries(NumT),
    findall(Surgery,surgery_id(Surgery, _),SurgeriesList),
    generate_population(PopSize,SurgeriesList,NumT,Pop).

generate_population(0,_,_,[]):-!.
generate_population(PopSize,SurgeriesList,NumT,[Ind|Rest]):-
    PopSize1 is PopSize-1,
    generate_population(PopSize1,SurgeriesList,NumT,Rest),
    generate_individual(SurgeriesList,NumT,Ind),
    not(member(Ind,Rest)).
generate_population(PopSize,SurgeriesList,NumT,L):-
    generate_population(PopSize,SurgeriesList,NumT,L).
    


generate_individual([G],1,[G]):-!.

generate_individual(SurgeriesList,NumT,[G|Rest]):-
    NumTemp is NumT + 1, % to use with random
    random(1,NumTemp,N),
    remove(N,SurgeriesList,G,NewList),
    NumT1 is NumT-1,
    generate_individual(NewList,NumT1,Rest).

remove(1,[G|Rest],G,Rest).
remove(N,[G1|Rest],G,[G1|Rest1]):- N1 is N-1,
            remove(N1,Rest,G,Rest1).


evaluate_population([], []).
evaluate_population([Ind | Rest], [Ind*V | Rest1]) :-
    % evaluate(Ind, V),
    day(Day),
    room(Room),
    (
        availability_all_surgeries(Ind, Room, Day) -> 
        (
            % Case when availability_all_surgeries succeeds
            agenda_operation_room1(Room, Day, Agenda),
            reverse(Agenda, Agenda1),
            % Agenda1 = [(1000,1059,so099999), (520,579,so100000)]
            get_last_time(Ind, Agenda1, V)
        )
        ;
        (
            % Case when availability_all_surgeries fails
            V is 1441
        )
    ),
    retractall(agenda_staff1(_,_,_)),
    retractall(agenda_operation_room1(_,_,_)),
    retractall(availability(_,_,_)),
    findall(_,(agenda_staff(D,Day,Agenda),assertz(agenda_staff1(D,Day,Agenda))),_),
    agenda_operation_room(Room,Day,Agenda2),
    assert(agenda_operation_room1(Room,Day,Agenda2)),
    findall(_,(agenda_staff1(D,Day,L),free_agenda0(L,LFA),adapt_timetable(D,Day,LFA,LFA2),assertz(availability(D,Day,LFA2))),_),

    evaluate_population(Rest, Rest1).

get_last_time([], _, _).
get_last_time(Ind, [(_, V, ID) | _], V) :-
    member(ID, Ind),         
    !.                       
get_last_time(Ind, [_ | Rest], V) :-
    get_last_time(Ind, Rest, V).

order_population(PopValue,PopValueOrd):-
    bsort(PopValue,PopValueOrd).

bsort([X],[X]):-!.
bsort([X|Xs],Ys):-
    bsort(Xs,Zs),
    bchange([X|Zs],Ys).


bchange([X],[X]):-!.

bchange([X*VX,Y*VY|L1],[Y*VY|L2]):-
    VX>VY,!,
    bchange([X*VX|L1],L2).

bchange([X|L1],[X|L2]):-bchange(L1,L2).
   

generate_generation(G,G,Pop):-!,
	write('Generation '), write(G), write(':'), nl, write(Pop), nl,nl,
    get_time(Now),
    start_time(Start),
    Time is Now - Start,
    write('Time: '), write(Time), write(' seconds'), nl,
    
    Pop = [_*V | _],
    write('Finish last surgery: '), write(V), write(' minutes.'), nl.

generate_generation(N, G, Pop) :-
    write('Generation '), write(N), write(':'), nl, write(Pop), nl,

    % Check if the best individual has reached the minimum fitness
    min_fitness(Min),
    Pop = [Ind*V | _],
    (V =< Min ->
        !,
        write('Best individual: '), write(Ind), write(' with fitness '), write(V), nl,
        write('Finish last surgery: '), write(V), write(' minutes.'), nl,
        true
    ; 

    % Check if the time limit has been reached
    time_limit(TL),
    get_time(Now),
    start_time(Start),
    Time is Now - Start,
    (Time > TL ->
        !,
        write('Time limit reached after '), write(Time), write(' seconds.'), nl,
        write('Finish last surgery: '), write(V), write(' minutes.'), nl,
        true
    ; 

    random_permutation(Pop, PopRandom),
    crossover(PopRandom, NPop1),
    mutation(NPop1, NPop),

    day(Day),
    room(Room),
    retractall(agenda_staff1(_,_,_)),
    retractall(agenda_operation_room1(_,_,_)),
    retractall(availability(_,_,_)),
    findall(_,(agenda_staff(D,Day,Agenda),assertz(agenda_staff1(D,Day,Agenda))),_),
    agenda_operation_room(Room,Day,Agenda),assert(agenda_operation_room1(Room,Day,Agenda)),
    findall(_,(agenda_staff1(D,Day,L),free_agenda0(L,LFA),adapt_timetable(D,Day,LFA,LFA2),assertz(availability(D,Day,LFA2))),_),
    evaluate_population(NPop, NPopValue),

    order_population(NPopValue, NPopOrd),

    selection_method(Pop, NPopOrd, NPopOrd1),
    order_population(NPopOrd1, NPopOrd2),

    % Check if the population has stabilized
    check_stabilize(N, Pop, NPopOrd2),
    count_stable_generations(CSG),
    stabilize_generations(SG),
    (CSG = SG ->
        !,
        write('Stabilized after '), write(CSG), write(' generations.'), nl,
        write('Finish last surgery: '), write(V), write(' minutes.'), nl,
        true
    ; 

    N1 is N + 1,
    generate_generation(N1, G, NPopOrd2)
))).


check_stabilize(N, OldPop, NewPop) :-
    stabilize_generations(SG), % number of generations to stabilize
    (SG = 0 ->
        true
    ;
    (N > SG ->
        (OldPop == NewPop ->
            retract(count_stable_generations(CSG)),
            CSG1 is CSG + 1,
            asserta(count_stable_generations(CSG1))
        ;
            retract(count_stable_generations(_)),
            asserta(count_stable_generations(0))
        ),
        true
    ;
        true
    )
).


selection_method(OldPop, NewPop, FinalList):-
    append(OldPop, NewPop, AllPop1),
    sort(AllPop1, AllPop), % remove duplicates

    order_population(AllPop, AllPopOrd),
    P is 20 / 100, % 20% of the population is selected
    length(AllPopOrd, Len),
    N is round(Len * P),

    sublist(AllPopOrd, 1, N, Top20Pop1),
    removeh(Top20Pop1, Top20Pop),

    sublist(AllPopOrd, N+1, Len, Bottom80Pop2),
    removeh(Bottom80Pop2, Bottom80Pop),

    associate_0_to_1(Bottom80Pop, Bottom80Pop1),

    order_population(Bottom80Pop1, Bottom80PopOrd),

    population(PopSize),
    N1 is PopSize - N,

    % Bottom80Pop -> normal
    % Bottom80PopOrd -> normal * [0..1]
    assign_correct_weight(Bottom80Pop, Bottom80PopOrd, Rest),

    sublist(Rest, 1, N1, TopBottom80Pop1),
    removeh(TopBottom80Pop1, TopBottom80Pop),
    
    append(Top20Pop, TopBottom80Pop, FinalList).


assign_correct_weight([], _, []).
assign_correct_weight([Ind*V|Rest], List2, [Ind*V|Result]):-
    member(Ind*_, List2),
    assign_correct_weight(Rest, List2, Result).
 
associate_0_to_1([],[]).
associate_0_to_1([Ind*V|Rest],[Ind*V1|Rest1]):-
    random(R),
    V1 is V*R,
    associate_0_to_1(Rest,Rest1).

generate_crossover_points(P1,P2):- generate_crossover_points1(P1,P2).

generate_crossover_points1(P1,P2):-
	surgeries(N),
	NTemp is N+1,
	random(1,NTemp,P11),
	random(1,NTemp,P21),
	P11\==P21,!,
	((P11<P21,!,P1=P11,P2=P21);P1=P21,P2=P11).
generate_crossover_points1(P1,P2):-
	generate_crossover_points1(P1,P2).


crossover([ ],[ ]).
crossover([Ind*_],[Ind]).
crossover([Ind1*_,Ind2*_|Rest],[NInd1,NInd2|Rest1]):-
	generate_crossover_points(P1,P2),
	prob_crossover(Pcruz),random(0.0,1.0,Pc),
	((Pc =< Pcruz,!,
        cross(Ind1,Ind2,P1,P2,NInd1),
	  cross(Ind2,Ind1,P1,P2,NInd2))
	;
	(NInd1=Ind1,NInd2=Ind2)),
	crossover(Rest,Rest1).

fillh([ ],[ ]).

fillh([_|R1],[h|R2]):-
	fillh(R1,R2).

sublist(L1,I1,I2,L):-I1 < I2,!,
    sublist1(L1,I1,I2,L).

sublist(L1,I1,I2,L):-sublist1(L1,I2,I1,L).

sublist1([X|R1],1,1,[X|H]):-!, fillh(R1,H).

sublist1([X|R1],1,N2,[X|R2]):-!,N3 is N2 - 1,
	sublist1(R1,1,N3,R2).

sublist1([_|R1],N1,N2,[h|R2]):-N3 is N1 - 1,
		N4 is N2 - 1,
		sublist1(R1,N3,N4,R2).

rotate_right(L,K,L1):- surgeries(N),
	T is N - K,
	rr(T,L,L1).

rr(0,L,L):-!.

rr(N,[X|R],R2):- N1 is N - 1,
	append(R,[X],R1),
	rr(N1,R1,R2).

remove([],_,[]):-!.

remove([X|R1],L,[X|R2]):- not(member(X,L)),!,
        remove(R1,L,R2).

remove([_|R1],L,R2):-
    remove(R1,L,R2).

insert([],L,_,L):-!.
insert([X|R],L,N,L2):-
    surgeries(T),
    ((N>T,!,N1 is N mod T);N1 = N),
    insert1(X,N1,L,L1),
    N2 is N + 1,
    insert(R,L1,N2,L2).


insert1(X,1,L,[X|L]):-!.
insert1(X,N,[Y|L],[Y|L1]):-
    N1 is N-1,
    insert1(X,N1,L,L1).

cross(Ind1,Ind2,P1,P2,NInd11):-
    sublist(Ind1,P1,P2,Sub1),
    surgeries(NumT),
    R is NumT-P2,
    rotate_right(Ind2,R,Ind21),
    remove(Ind21,Sub1,Sub2),
    P3 is P2 + 1,
    insert(Sub2,Sub1,P3,NInd1),
    removeh(NInd1,NInd11).


removeh([],[]).

removeh([h|R1],R2):-!,
    removeh(R1,R2).

removeh([X|R1],[X|R2]):-
    removeh(R1,R2).

mutation([],[]).
mutation([Ind|Rest],[NInd|Rest1]):-
	prob_mutation(Pmut),
	random(0.0,1.0,Pm),
	((Pm < Pmut,!,mutacao1(Ind,NInd));NInd = Ind),
	mutation(Rest,Rest1).

mutacao1(Ind,NInd):-
	generate_crossover_points(P1,P2),
	mutacao22(Ind,P1,P2,NInd).

mutacao22([G1|Ind],1,P2,[G2|NInd]):-
	!, P21 is P2-1,
	mutacao23(G1,P21,Ind,G2,NInd).
mutacao22([G|Ind],P1,P2,[G|NInd]):-
	P11 is P1-1, P21 is P2-1,
	mutacao22(Ind,P11,P21,NInd).

mutacao23(G1,1,[G2|Ind],G2,[G1|Ind]):-!.
mutacao23(G1,P,[G|Ind],G2,[G|NInd]):-
	P1 is P-1,
	mutacao23(G1,P1,Ind,G2,NInd).

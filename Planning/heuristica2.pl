
:- dynamic availability/3.
:- dynamic agenda_staff/3.
:- dynamic agenda_staff1/3.
:-dynamic agenda_operation_room/3.
:-dynamic agenda_operation_room1/3.
:-dynamic better_sol/5.


%%
% For the Complexity Study

%agenda_staff(d001,20241028,[(720,790,m01),(1080,1140,c01)]).
%agenda_staff(d002,20241028,[(850,900,m02),(901,960,m02),(1380,1440,c02)]).
%agenda_staff(d003,20241028,[(720,790,m01),(910,980,m02)]).
%agenda_staff(d004,20241028,[(850,900,m02),(940,980,c04)]).

%timetable(d001,20241028,(480,1200)).
%timetable(d002,20241028,(500,1440)).
%timetable(d003,20241028,(520,1320)).
%timetable(d004,20241028,(620,1020)).

%surgery_id(so100001,so2).
%surgery_id(so100002,so3).
%surgery_id(so100003,so4).
%surgery_id(so100004,so2).
%surgery_id(so100005,so4).
%surgery_id(so100006,so2).
%surgery_id(so100007,so3).
%surgery_id(so100008,so2).
%surgery_id(so100009,so2).
%surgery_id(so100010,so2).
%surgery_id(so100011,so4).
%surgery_id(so100012,so2).
%surgery_id(so100013,so2).

%assignment_surgery(so100001,d001).
%assignment_surgery(so100002,d002).
%assignment_surgery(so100003,d003).
%assignment_surgery(so100004,d001).
%assignment_surgery(so100004,d002).
%assignment_surgery(so100005,d002).
%assignment_surgery(so100005,d003).
%assignment_surgery(so100006,d001).
%assignment_surgery(so100007,d003).
%assignment_surgery(so100008,d004).
%assignment_surgery(so100008,d003).
%assignment_surgery(so100009,d002).
%assignment_surgery(so100009,d004).
%assignment_surgery(so100010,d003).
%assignment_surgery(so100011,d001).
%assignment_surgery(so100012,d001).
%assignment_surgery(so100013,d004).


agenda_operation_room(or1,20241028,[(750, 780, mnt0001),(1080, 1110, mnt0002)]).


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




schedule_all_surgeries(Room,Day):-
    retractall(agenda_staff1(_,_,_)),
    retractall(agenda_operation_room1(_,_,_)),
    retractall(availability(_,_,_)),
    findall(_,(agenda_staff(D,Day,Agenda),assertz(agenda_staff1(D,Day,Agenda))),_),
    agenda_operation_room(Or,Date,Agenda),assert(agenda_operation_room1(Or,Date,Agenda)),
    findall(_,(agenda_staff1(D,Date,L),free_agenda0(L,LFA),adapt_timetable(D,Date,LFA,LFA2),assertz(availability(D,Date,LFA2))),_),
    findall(OpCode,surgery_id(OpCode,_),LOpCode),

    availability_all_surgeries(LOpCode,Room,Day),!.

availability_all_surgeries([],_,_).
availability_all_surgeries([OpCode|LOpCode],Room,Day):-
    surgery_id(OpCode,OpType),surgery(OpType,_,TSurgery,_),
    availability_operation(OpCode,Room,Day,LPossibilities,LDoctors),
    schedule_first_interval(TSurgery,LPossibilities,(TinS,TfinS)),
    retract(agenda_operation_room1(Room,Day,Agenda)),
    insert_agenda((TinS,TfinS,OpCode),Agenda,Agenda1),
    assertz(agenda_operation_room1(Room,Day,Agenda1)),
    insert_agenda_doctors((TinS,TfinS,OpCode),Day,LDoctors),
    availability_all_surgeries(LOpCode,Room,Day).



availability_operation(OpCode,Room,Day,LPossibilities,LDoctors):-surgery_id(OpCode,OpType),surgery(OpType,_,TSurgery,_),
    findall(Doctor,assignment_surgery(OpCode,Doctor),LDoctors),
    intersect_all_agendas(LDoctors,Day,LA),
    agenda_operation_room1(Room,Day,LAgenda),
    free_agenda0(LAgenda,LFAgRoom),
    intersect_2_agendas(LA,LFAgRoom,LIntAgDoctorsRoom),
    remove_unf_intervals(TSurgery,LIntAgDoctorsRoom,LPossibilities).


remove_unf_intervals(_,[],[]).
remove_unf_intervals(TSurgery,[(Tin,Tfin)|LA],[(Tin,Tfin)|LA1]):-DT is Tfin-Tin+1,TSurgery=<DT,!,
    remove_unf_intervals(TSurgery,LA,LA1).
remove_unf_intervals(TSurgery,[_|LA],LA1):- remove_unf_intervals(TSurgery,LA,LA1).


schedule_first_interval(TSurgery,[(Tin,_)|_],(Tin,TfinS)):-
    TfinS is Tin + TSurgery - 1.

insert_agenda((TinS,TfinS,OpCode),[],[(TinS,TfinS,OpCode)]).
insert_agenda((TinS,TfinS,OpCode),[(Tin,Tfin,OpCode1)|LA],[(TinS,TfinS,OpCode),(Tin,Tfin,OpCode1)|LA]):-TfinS<Tin,!.
insert_agenda((TinS,TfinS,OpCode),[(Tin,Tfin,OpCode1)|LA],[(Tin,Tfin,OpCode1)|LA1]):-insert_agenda((TinS,TfinS,OpCode),LA,LA1).

insert_agenda_doctors(_,_,[]).
insert_agenda_doctors((TinS,TfinS,OpCode),Day,[Doctor|LDoctors]):-
    retract(agenda_staff1(Doctor,Day,Agenda)),
    insert_agenda((TinS,TfinS,OpCode),Agenda,Agenda1),
    assert(agenda_staff1(Doctor,Day,Agenda1)),
    insert_agenda_doctors((TinS,TfinS,OpCode),Day,LDoctors).



obtain_better_sol(Room,Day,AgOpRoomBetter,LAgDoctorsBetter,TFinOp):-
		get_time(Ti),
		(obtain_better_sol1(Room,Day);true),
		retract(better_sol(Day,Room,AgOpRoomBetter,LAgDoctorsBetter,TFinOp)),
            write('Final Result: AgOpRoomBetter='),write(AgOpRoomBetter),nl,
            write('LAgDoctorsBetter='),write(LAgDoctorsBetter),nl,
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
             write('now: FinTime1='),write(FinTime1),write(' Agenda='),write(Agenda),nl,
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



% Heuristica 2--------------------------------------------------------------

% Encontra e processa a melhor solução para operações em uma sala específica de cirurgia em um determinado dia. \5
heuristica2(Room, Day, AgOpRoomBetter, LAgDoctorsBetter, TFinOp):-
    get_time(Ti),                                                       % Inicia a contagem tempo                                                                                   
    (obtain_better_sol2(Room, Day); true),                              % Encontra a melhor solução possivel para as cirurgias em uma sala para o dia especificado
    retract(better_sol(Day, Room, AgOpRoomBetter, LAgDoctorsBetter, _)), % Recupera a melhor solução encontrada
    calculo_tempo_final(AgOpRoomBetter, LAgDoctorsBetter, TFinOp),      % Calcula o tempo final da última cirurgia realizada
    get_time(Tf),                                                       % Finaliza a contagem tempo                             
    T is Tf - Ti,                                                       % Calcula o tempo total
    write('Solution Time:'), write(T), nl.                              % Exibe o tempo total

% Calcula o tempo final da última cirurgia realizada. \3
calculo_tempo_final(AgOpRoom, LAgDoctors, TFinOp):-                                 % Calcula o tempo final da ultima cirurgia realizada
  findall(Tfin,                                                                     % Para cada tempo final
    (
      member((_, Tfin, OpCode), AgOpRoom),                                          % Verifica a operação realizada
      member((_, AgendaDoctor), LAgDoctors),                                        % Verifica a agenda doutor
      member((_, _, OpCode), AgendaDoctor)                                          % Verifica a operação realizada pelo doutor                  
    ), ListaFins),                                                                  % Lista de tempos finais
  (ListaFins = [] -> TFinOp = 0 ; max_list(ListaFins, TFinOp)).                     % Se a lista de tempos finais estiver vazia, o tempo final e 0, senão, o tempo final e o maior tempo final da lista

% Busca a melhor solução possível para as cirurgias em uma sala no dia especificado. \2
obtain_better_sol2(Room,Day):- 
    asserta(better_sol(Day,Room,_,_,1441)),                                                                                          % Inicializa a melhor solução possivel
    findall(OpCode,surgery_id(OpCode,_),LOC),!,                                                                                      % Encontra todas as operações possiveis
    permutation(LOC,LOpCode),                                                                                                        % Permuta as operações possiveis               
    retractall(agenda_staff1(_,_,_)),                                                                                                % Remove todas as agendas dos doutores
    retractall(agenda_operation_room1(_,_,_)),                                                                                       % Remove todas as agendas das salas de cirurgia
    retractall(availability(_,_,_)),                                                                                                 % Remove todas as disponibilidades                      
    findall(_,(agenda_staff(D,Day,Agenda),assertz(agenda_staff1(D,Day,Agenda))),_),                                                  % Adiciona as agendas dos doutores
    agenda_operation_room(Room,Day,Agenda),assert(agenda_operation_room1(Room,Day,Agenda)),                                          % Adiciona as agendas das salas de cirurgia
    findall(_,(agenda_staff1(D,Day,L),free_agenda0(L,LFA),adapt_timetable(D,Day,LFA,LFA2),assertz(availability(D,Day,LFA2))),_),     % Adiciona as disponibilidades
    availability_all_surgeries(LOpCode,Room,Day),                                                                                    % Verifica a disponibilidade de todas as cirurgias
    agenda_operation_room1(Room,Day,AgendaR),                                                                                        % Recupera a agenda da sala de cirurgia
		update_better_sol2(Day,Room,AgendaR,LOpCode),                                                                                % Atualiza a melhor solução
		fail.                                                                                                                        % Falha

% Atualiza a melhor solução possível para as cirurgias em uma sala no dia especificado. \4
update_better_sol2(Day, Room, Agenda, LOpCode):-                                                                                    % Atualiza a melhor solução possível
    better_sol(Day, Room, _, _, BestPercentage),                                                                                    % Recupera a melhor solução possível
    occupation_doctor_max_percentage(Day, LOpCode, MaxPercentage),                                                                  % Calcula a ocupaçao maxima dos doutores
    (MaxPercentage < BestPercentage ->                                                                                              % Se a ocupaçao maxima e menor que a ocupaçao maxima da melhor soluçao                                      
        retract(better_sol(_, _, _, _, _)),                                                                                         % Remove a melhor soluçao
        findall(Doctor, assignment_surgery(_, Doctor), LDoctors1),                                                                  % Encontra todos os doutores
        remove_equals(LDoctors1, LDoctors),                                                                                         % Remove os doutores iguais
        list_doctors_agenda(Day, LDoctors, LDAgendas),                                                                              % Lista as agendas dos doutores
        asserta(better_sol(Day, Room, Agenda, LDAgendas, MaxPercentage)),                                                           % Adiciona a melhor soluçao
        write('Best Solution (second heuristic): '),nl, write('Max(%): = '), write(MaxPercentage), nl                               % Exibe a melhor soluçao
    ; true).                                                                                                                        

% Calcula a ocupaçao maxima dos doutores. \3
occupation_doctor_max_percentage(Day, LOpCode, MaxPercentage):-                                                                                     
    findall(Percentage, (member(OpCode, LOpCode), assignment_surgery(OpCode, Medico), occupation_staff(Medico, Day, Percentage)), ListPercentage),  
    max_list(ListPercentage, MaxPercentage).                                                                                        

% Calcula a ocupaçao dos doutores em percentagem \3
occupation_staff(Medico, Dia, Percentage):-                             
    agenda_staff1(Medico, Dia, Agenda),                                 % Recupera a agenda do doutor
    staff_occupation_time(Agenda, TotalTimeBusy),                       % Calcula o tempo ocupado
    timetable(Medico, Dia, (Inicio, Fim)),                              % Recupera o tempo disponivel
    TempoDisponivel is Fim - Inicio,                                    % Calcula o tempo disponivel
    Percentage is (TotalTimeBusy / TempoDisponivel) * 100.              % Calcula a porcentagem de tempo ocupado

% Calcula o tempo ocupado dos doutores. \2
staff_occupation_time([], 0).                                           
staff_occupation_time([(Tin, Tfin, _) | Resto], TotalTimeBusy):-        
    Duracao is Tfin - Tin,                                                                   
    staff_occupation_time(Resto, TempoResto),                           
    TotalTimeBusy is Duracao + TempoResto.              

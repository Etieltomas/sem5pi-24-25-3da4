:- use_module(library(http/http_client)).
:- use_module(library(http/json)).

%  (RoomNumber, Capacity, AssignedEquipment, RoomStatus, MaintenanceSlot, Type)
room_dto(_, _, _, _, _, _).

http_handler('/api/Room', get_room(ID), [id(ID), method(get)]).

% Make a GET request to the API and convert the response into a list of RoomDTOs
get_room(ID) :-
    atom_concat('http://localhost:5012/api/Room/', ID, URL),
    
    % Perform the GET request
    http_get(URL, Response, []),
    
    % Parse the response as a JSON single room
    atom_json_dict(Response, RoomDict, []),
    convert_to_room_dto(RoomDict, RoomDTO),
    print_room_dtos([RoomDTO]).
    

convert_to_room_dto(Dict, room_dto(RoomNumber, Capacity, AssignedEquipment, RoomStatus, MaintenanceSlot, Type)) :-
    _{roomNumber: RoomNumber, capacity: Capacity, assignedEquipment: AssignedEquipment, 
      roomStatus: RoomStatus, maintenanceSlot: MaintenanceSlot, type: Type} :< Dict.

print_room_dtos([]).
print_room_dtos([RoomDTO | Rest]) :-
    format('~w~n', [RoomDTO]),  % Print each RoomDTO on a new line
    print_room_dtos(Rest).  % Recur for the rest of the list

% Definição do DTO: (OperationRequestID, PatientName, OperationType, Priority, Deadline, StartDate, EndDate, Status)
operation_request_dto(_, _, _, _, _, _, _, _).

http_handler('/api/OperationRequest/Scheduled', get_scheduled_operations, [method(get)]).

% Faz um GET para a API e filtra apenas OperationRequest com status "scheduled"
get_scheduled_operations :-
    URL = 'http://localhost:5012/api/OperationRequest',
    
    % Faz o GET
    http_get(URL, Response, []),
    
    % Converte a resposta JSON em uma lista de dicionários
    atom_json_dict(Response, OperationList, []),
    
    % Filtra apenas as operações com status "scheduled"
    include(is_scheduled, OperationList, ScheduledOperations),
    
    % Converte para DTOs
    maplist(convert_to_operation_request_dto, ScheduledOperations, ScheduledDTOs),
    
    % Exibe os DTOs
    print_operation_dtos(ScheduledDTOs).

is_scheduled(Operation) :-
    Operation.status == "scheduled".

convert_to_operation_request_dto(Dict, operation_request_dto(ID, PatientName, OperationType, Priority, Deadline, StartDate, EndDate, Status)) :-
    _{id: ID, patientName: PatientName, operationType: OperationType, priority: Priority, deadline: Deadline, 
      startDate: StartDate, endDate: EndDate, status: Status} :< Dict.

print_operation_dtos([]).
print_operation_dtos([OperationDTO | Rest]) :-
    format('~w~n', [OperationDTO]),  % Exibe cada DTO
    print_operation_dtos(Rest).      % Recursão para o restante da lista


% Definição do DTO: (StaffID, Name, Email, Phone, Specialization, AvailabilitySlots)
staff_dto(_, _, _, _, _, _).

% Definição do DTO: (OperationRequestID, PatientName, OperationType, Priority, Deadline, StartDate, EndDate, Status)
operation_request_dto(_, _, _, _, _, _, _, _).

% HTTP handler para buscar staffs disponíveis
http_handler('/api/Staff/Available', get_available_staffs, [method(get)]).
http_handler('/api/OperationRequest/Scheduled', get_scheduled_operations, [method(get)]).

% Faz um GET para buscar staffs disponíveis
get_available_staffs :-
    URL = 'http://localhost:5012/api/Staff/Available',

    % Faz o GET
    http_get(URL, Response, []),

    % Converte a resposta JSON em uma lista de dicionários
    atom_json_dict(Response, StaffList, []),

    % Converte para DTOs
    maplist(convert_to_staff_dto, StaffList, StaffDTOs),

    % Exibe os DTOs
    print_staff_dtos(StaffDTOs).

convert_to_staff_dto(Dict, staff_dto(ID, Name, Email, Phone, Specialization, AvailabilitySlots)) :-
    _{id: ID, name: Name, email: Email, phone: Phone, specialization: Specialization,
      availabilitySlots: AvailabilitySlots} :< Dict.

print_staff_dtos([]).
print_staff_dtos([StaffDTO | Rest]) :-
    format('~w~n', [StaffDTO]),  % Exibe cada DTO
    print_staff_dtos(Rest).      % Recursão para o restante da lista
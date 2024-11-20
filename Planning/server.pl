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

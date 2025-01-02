agenda_staff(d001,20241028,[]).
agenda_staff(d002,20241028,[]).
agenda_staff(d003,20241028,[]).
agenda_staff(d004,20241028,[]).
agenda_staff(n001,20241028,[]).
agenda_staff(n002,20241028,[]).
agenda_staff(n003,20241028,[]).
agenda_staff(a001,20241028,[]).

timetable(d001,20241028,(480,1200)).
timetable(d002,20241028,(500,1440)).
timetable(d003,20241028,(520,1320)).
timetable(d004,20241028,(480,1220)).
timetable(n001,20241028,(480,1320)).
timetable(n002,20241028,(480,1120)).
timetable(n003,20241028,(480,1120)).
timetable(a001,20241028,(480,1220)).

staff(d001,doctor,orthopaedist).
staff(d002,doctor,orthopaedist).
staff(d003,doctor,orthopaedist).
staff(d004,doctor,anaesthetist).
staff(n001,nurse, instrumenting).
staff(n002,nurse, circulating).
staff(n003,nurse, anaesthetist).
staff(a001,assistant, action).

%surgery(SurgeryType,TAnesthesia,TSurgery,TCleaning).
surgery(so2,15,20,15).
surgery(so3,15,30,15).
surgery(so4,15,40,15).

surgery_id(so100001,so2).
surgery_id(so100003,so4).
surgery_id(so100002,so3).
surgery_id(so100004,so4).
surgery_id(so100005,so3).
surgery_id(so100006,so4).
surgery_id(so100007,so4).

assignment_surgery(so100001,d001).
assignment_surgery(so100001,d002).
assignment_surgery(so100001,n001).
assignment_surgery(so100001,n002).
assignment_surgery(so100001,n003).
assignment_surgery(so100001,a001).
assignment_surgery(so100001,d004).

assignment_surgery(so100002,d002).
assignment_surgery(so100002,d003).
assignment_surgery(so100002,n001).
assignment_surgery(so100002,n002).
assignment_surgery(so100002,n003).
assignment_surgery(so100002,a001).
assignment_surgery(so100002,d004).

assignment_surgery(so100003,d003).
assignment_surgery(so100003,n001).
assignment_surgery(so100003,n002).
assignment_surgery(so100003,n003).
assignment_surgery(so100003,a001).
assignment_surgery(so100003,d004).

assignment_surgery(so100004,d001).
assignment_surgery(so100004,n001).
assignment_surgery(so100004,n002).
assignment_surgery(so100004,n003).
assignment_surgery(so100004,a001).
assignment_surgery(so100004,d004).

assignment_surgery(so100005,d002).
assignment_surgery(so100005,n001).
assignment_surgery(so100005,n002).
assignment_surgery(so100005,n003).
assignment_surgery(so100005,a001).
assignment_surgery(so100005,d004).

assignment_surgery(so100006,d003).
assignment_surgery(so100006,n001).
assignment_surgery(so100006,n002).
assignment_surgery(so100006,n003).
assignment_surgery(so100006,a001).
assignment_surgery(so100006,d004).

assignment_surgery(so100007,d002).
assignment_surgery(so100007,n001).
assignment_surgery(so100007,n002).
assignment_surgery(so100007,n003).
assignment_surgery(so100007,a001).
assignment_surgery(so100007,d004).


%agenda_operation_room(or1,20241028,[(520,579,so100000),(1000,1059,so099999)]).
agenda_operation_room(or1,20241028,[(0,700,so099999),(900,1440,so099999)]).
agenda_operation_room(or2,20241028,[(0,900,so099999),(1222,1440,so099999)]).
agenda_operation_room(or3,20241028,[(0,600,so099999), (800,1400,so099999)]).

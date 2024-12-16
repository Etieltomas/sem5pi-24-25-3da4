:- dynamic agenda_staff/3.
:- dynamic timetable/3.
:- dynamic surgery/4.
:- dynamic surgery_id/2.
:- dynamic staff/3.
:- dynamic assignment_surgery/2.
:- dynamic agenda_operation_room/3.

agenda_staff(t202400001,20251010,[]).
agenda_staff(s202400001,20251010,[]).
agenda_staff(d202400001,20251010,[]).

timetable(t202400001,20251010,(420,1080)).
timetable(d202400001,20251010,(420,1080)).
timetable(s202400001,20251010,(420,1080)).

surgery(1,15,20,15).

surgery_id(11,1).
surgery_id(12,1).

assignment_surgery(11,t202400001).
assignment_surgery(11,s202400001).
assignment_surgery(11,d202400001).
assignment_surgery(12,t202400001).
assignment_surgery(12,s202400001).
assignment_surgery(12,d202400001).

staff(t202400001,doctor,orthopaedist).
staff(s202400001,doctor,anaesthetist).
staff(d202400001,assistant,orthopaedist).

agenda_operation_room(9,20251010,[]).
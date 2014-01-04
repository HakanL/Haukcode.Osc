
INTRO 

This is a simple project to demo how to use Rug.Osc from within unity, 
it is not ment as a best practice or a complete solution. 

PLEASE NOTE! 

You need to set the Player Optimization Api Compatibility Level for Rug.Osc to work with
Unity! You will find the option here: 

Edit -> Project Settings -> Player
Other Settings -> Optimization -> Api Compatibility Level -> .NET 2.0



THE DEMO

Basicly the sphere is linked to the cube via osc, when the cube moves so does the sphere. 


How this is done: 
 
The scene object "__Osc Controller" has 2 behaviours ascociated with it
 - OscReceiveController
 - OscSendController 

These are OSC send and receive sockets, they are connected on the Awake event


The scene object "CubeSender" has 1 custom behaviour ascociated with it. 
 - OscPositionSender
 
OscPositionSender extends the base behaviour SendOscBehaviourBase is a simple hook onto 
the OscSendController behaviour on "__Osc Controller" all it is doing is simplifiying the 
creation of custom senders by exposing a "Send" method. 

OscPositionSender provides the custom functionality, on every Update event it checks the 
current position of the cube agaist the last position and if they are different sends the 
current postition via an OSC message. 


The scene object "SphereReceiver" has 1 custom behaviour ascociated with it. 
 - OscPositionReceiver
 
Like OscPositionSender OscPositionReceiver extends a base behaviour in this case 
ReceiveOscBehaviourBase. ReceiveOscBehaviourBase just automates the hooking up to the 
OscReceiveController behaviour on "__Osc Controller", again all it is doing is simplifiying 
the creation of custom receivers by attaching the abstract "ReceiveMessage" method to a 
specific osc addresses (literal or pattern). 

OscPositionSender simply sets the position of the "SphereReceiver" object to that of the 
recived message plus the "Offset" property


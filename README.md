First Screen Evaluation Task Solution
--------------------------------------------------
Candidate Name: Jamsheed BP

Enclosed Solution A, B and C

Application A 
	A is the listener/server application which create an xml file with random records of type Red, Blue and Green. 
The application help can be seen by running " A --help". The app A have 2 optional command line arguments, first for the app 
run mode(fast or slow) and the second argument is to specify the number of xml records need to be generated. 
If not provided the default value will be used, the default run mode is "slow" and default xml record count is "1000". Once the 
app is running you will be asked to provide the listener address(eg:127.0.0.1:8888), provide the ip:port to be binded for the
TCP connection. Now the app will be listening for connection request and will serve the records for the specified type.

 Running Instructions
	Type A <optional:mode[fast/slow]> <optional:xmlrecords[number]>
	When Prompted provide <ip>:<port>

 Architecture 
	When started app A first creates an XML Input file in a predefined destination then it starts a new thread for listenining
to new connection request. Next it filters out xml based on record type and spawns a new thread for each record type. And thus when
a new connection is binded data will be passed to the client of registered type.


Application B
	App B is the client app which can have multiple instance. The app should be run using a required command line argument
server address(App A) to connect to App A. Then it will be prompted to make a choice on record type. 

	
	Running Instructions
	Type B <required:address[<ip>:<port>]>
	When Prompted provide choice


Application C
	This is a web app which communicate to server app A and displays data in web page. In web.config file provide the 
address of the Server app A under the key "ListenerAddress".

Architecture 
	WebApp (C) uses ASP.NET MVC and SignalR for realtime websocket communication. C and B utilises the same architecture for
the communication with App A. Since this functionalities are shared I have used a common dll(NetworkUtilities.dll) to stick with 
the DRY principle.


 Remarks
This solution doesn't have any unit test cases, Since time was a constraint


	





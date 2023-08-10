
# UniversalMultiplayer

THIS PROJECT IS NOT DONE YET!!!
(You can still try to use it tho)

This is just the client; the server is in this git repository: 
https://github.com/theradest1/Project-B-Server-2/tree/no-call-response

My plan for this is to make a very basic and flexible multiplayer that works with unity. This would make it very easy to just duplicate this project and start from there instead of making a new server every time. It also will allow other programmers to use or reference the code as an example.

________

How to use:

Enter in the server ip and port into the client script (Ip is 127.0.0.1 localally hosted). Remember to port forward the server if you haven't already. 

Start the server by cloning the given git repo linked above, and running in visual studio. I will turn it into an executable in the future. (It will be in releases)

Then run to connect, edit CustomEvents.cs in unity to add events (follow the example)

Don't be afraid to ask for help (:

______

Multiplayer specifications:

UDP is very fast along with not requiring as much bandwidth compared to TCP and HTTP, so it is used for constant updating game states such as player positions.

Since UDP sacrifices consistant messages for speed, TCP is needed for other things such as events. While generally demanding more bandwidth and slightly higher latency, it is the best way to have all messages reach the endpoint in the correct order. This is very important to make sure all clients are on the same page.

To make sure the server is extremely fast with very low latency, it is built in c++. It also stores very little data on-server, making it use extremely low amounts of memory. My end goal is to be able to run this server comfortably on a raspberry pi, a very slow but tiny computer.

There is a simple way of adding events that get dealt with automatically by the client, making it fast and easy to use. While making it easy, it also gives you just about total freedom to do what you want with it.

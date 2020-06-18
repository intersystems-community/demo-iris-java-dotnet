# Demonstration of using the .NET Gateway to allow ObjectScript and .NET to work together

This documents how to instantiate and interact with a C# object from ObjectScript in InterSystems IRIS.  This is provided for demonstration purposes and is not intended for production use.

## Building the source

This is a VisualStudio 2019 project.  

In the References section, you'll need to add/update the references pointing to InterSystems.Data.IRISClient and InterSystems.Data.Gateway64 to point to the libraries that came with your IRIS installation.  Also, make sure that you have the reference to Confluent.Kafka NuGet package.

Once your references are set, you should be able to build the project.  This should create an assembly which we will reference in the demos.

## Creating the Gateway

This demo requires that you have a .NET gateway named `DotNet Gateway` running on port `44444` configured on your IRIS instance. 

## Demo 1 - Basic interaction with a C# object

The first demo will load the assembly, create an example Fruit object and call a few methods on the object.  For these demos, you'll want to use the ObjectScript console in InterSystems IRIS.

Here is the script that was used in the demo.  Note that you'll need to adjust the path to the assembly to match where you built yours.

```
// Start the DotNet Gateway
set st = ##class(%Net.Remote.Service).OpenGateway("DotNet Gateway",.OG)
set status = ##class(%Net.Remote.Service).StartGateway(OG.Name)

// Load the assembly
set assemblyPath = ##class(%ListOfDataTypes).%New()
do assemblyPath.Insert("C:\Users\rkuszews\Iris\LanguageInterop\languageinterop-dotnet\example\example\bin\Debug\example.dll")

// Connect a Gateway instance to server "DotNet Gateway" on the host machine
set GW = ##class(%Net.Remote.Gateway).%New()
set st = GW.%Connect(OG.Server, OG.Port, "USER",,assemblyPath)

// Create the .NET object
set myObject = ##class(%Net.Remote.Object).%New(GW,"example.Fruit")

// Call the set/get functions on the object
do myObject.setFruit("Blueberry")
write !,"Fruit preference is: ",!,"   "_myObject.getFruit()

// When you're done with your app, shut down the gateway
set st = GW.%Disconnect()
set st =  ##class(%Net.Remote.Service).StopGatewayObject(OG)
```

## Demo 2 - Call back into IRIS from the C# object

The second demo requires a bit more setup, because it is doing something more complex.

### Install and configure Zookeeper and Kafka

This demo requires that you have access to a Kafka instance with a topic named `my-topic`.  If you don't have Kafka installed, please follow the online installation guides and create the topic.

### Create the table in IRIS

This demo expects that you have a SQL table named `Example.Messages` in your IRIS instance.  Go to the SQL tool and create it
```
CREATE TABLE Example.Messages (
  RecordID   IDENTITY NOT NULL,
  Body   NVARCHAR(2048) NOT NULL
)
```

### Run the demo

Similar to Demo 1, the easiest way to explore interacting with C# from ObjectScript is through the IRIS Console.  You'll need to adjust this one with the right paths.

```
// Start the gateway and load your assembly when you start your app
set st = ##class(%Net.Remote.Service).OpenGateway("DotNet Gateway",.OG)
set status = ##class(%Net.Remote.Service).StartGateway(OG.Name)

set assemblyPath = ##class(%ListOfDataTypes).%New()
do assemblyPath.Insert("C:\Users\rkuszews\Iris\LanguageInterop\languageinterop-dotnet\example\example\bin\Debug\example.dll")

// Connect a Gateway instance to server "DotNet Gateway" on the host machine
set GW = ##class(%Net.Remote.Gateway).%New()
set st = GW.%Connect(OG.Server, OG.Port, "USER",,assemblyPath)


// Create the Kafka object
set proxyKafka = ##class(%Net.Remote.Object).%New(GW,"example.kafka")

// Connect to Kafka and start listening for new messages on the topic
do proxyKafka.startConsumer("my-topic")

// At this point, you'll want to add some traffic to your topic

// Write all the recent messages to the Example.Messages table in IRIS
do proxyKafka.consumePendingMessages()

// When you're done, stop the consumer
do proxyKafka.stopConsumer()

set st = GW.%Disconnect()
set st =  ##class(%Net.Remote.Service).StopGatewayObject(OG)
```

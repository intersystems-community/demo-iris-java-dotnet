# InterSystems IRIS PEX Demo

This is a very basic example of a PEX BusinessOperation that writes a file to Amazon AWS S3.  It is provided for illustration purposes only and is not intended for production use.

## Building the source code

1. Make sure that maven is installed and added to your path

2. Add the two dependent InterSystems IRIS libraries via maven.  For example, this is a typical example for a Windows installation:

```
mvn install:install-file -Dfile=c:\InterSystems\IRIS\dev\java\lib\JDK18\intersystems-jdbc-3.1.0.jar -DgroupId=com.intersystems -DartifactId=intersystems-jdbc -Dversion=3.1.0 -Dpackaging=jar -DgeneratePom=true

mvn install:install-file -Dfile=c:\InterSystems\IRIS\dev\java\lib\JDK18\intersystems-gateway-3.1.0.jar -DgroupId=com.intersystems -DartifactId=intersystems-gateway -Dversion=3.1.0 -Dpackaging=jar -DgeneratePom=true
```

3. Build the demo via

```
mvn install clean compile package
```

That should create a single jar file, which will be referenced when we create the production.

## Authentication to AWS

In 'MyBusinessOperation.java', you can see that we build a standard AWS client: `AmazonS3ClientBuilder.standard().withRegion(Regions.US_WEST_2).build()`  This picks up your AWS credentials from the local environment.  See AWS's documentation for how to set this up.  https://docs.aws.amazon.com/sdk-for-java/v2/developer-guide/setup-credentials.html

## Setting up the Java Gateway

For this demo to work, you'll need to set up a Java Gateway.  You'll want to set the Class Path to include the AWS SDK Jars.  Here's what I included for the demo

* C:\MyApps\aws-java-sdk-1.11.802\lib\*
* C:\MyApps\aws-java-sdk-1.11.802\third-party\lib\*

## Creating the production

The demo consisted of just two steps - a BusinessService that monitors a directory for new files; and the custom BusinessOperation that writes the file to AWS S3.  Import `Webinar_s3demo-production.xml` to load the exact production used in the demo.  Please note that you'll need to change all the paths to match your setup.

* The DirectoryWatcher BusinessService expects to find files in `c:\MyApps\In` and has helper directories in `c:\MyApps`.  You can either create them yourself or change the business service to match the directory you want to use
* The SendToS3 BusinessOperation needs to be configured with the port of the Java gateway you set up above and with the full path to the JAR file you build above.


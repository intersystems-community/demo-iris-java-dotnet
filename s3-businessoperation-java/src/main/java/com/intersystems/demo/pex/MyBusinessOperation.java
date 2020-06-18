package com.intersystems.demo.pex;

import com.intersystems.jdbc.*;

import com.amazonaws.regions.Regions;
import com.amazonaws.services.s3.AmazonS3;
import com.amazonaws.services.s3.AmazonS3ClientBuilder;

public class MyBusinessOperation extends com.intersystems.enslib.pex.BusinessOperation {

	// Public member variables can be configured in the Production
	public String bucket;

	public void OnInit() throws Exception {
		// This is called when the production is started
		return;
	}

	public void OnTearDown() throws Exception {
		// Called when the production is stopped
		return;
	}
	
	public Object OnMessage(Object request) throws Exception {
		// Called whenever there is a message to be delivered.
		// request's type is agreed by convention from the caller

		LOGINFO("Processing Message...");

		// Let's extract the original filename, content size, and content from request
		IRISObject streamContainer = (IRISObject)request;
		IRISObject str = (IRISObject)streamContainer.get("Stream");
		String originalFilename = (String)streamContainer.get("OriginalFilename");

		Long contentSize = (Long)str.get("Size");
		String content = (String)str.invoke("Read", contentSize);

		LOGINFO("Uploading "+originalFilename+" which is "+contentSize+" in length to bucket "+bucket+" ...");

        final AmazonS3 s3 = AmazonS3ClientBuilder.standard().withRegion(Regions.US_WEST_2).build();
		s3.putObject(bucket, originalFilename, content);

		LOGINFO("   ... Done!");

		// returns an object that is returned to the caller
		return null;
	}

}
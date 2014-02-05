package net.pic4pic.ginger.tasks;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;

import org.apache.http.HttpResponse;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.entity.InputStreamEntity;

import android.content.Context;
import android.util.Log;

import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.ImageUploadRequest;
import net.pic4pic.ginger.entities.ImageUploadResponse;
import net.pic4pic.ginger.utils.GingerNetUtils;

public class ImageUploadTask extends BlockedTask<String, Void, ImageUploadResponse> {

	private ImageUploadRequest request;
	private ImageUploadListener listener;
	
	public ImageUploadTask(ImageUploadListener listener, Context context, ImageUploadRequest request){
		
		super(context);
		this.listener = listener;
		this.request = request;
	}
	
	@Override
    protected ImageUploadResponse doInBackground(String... execArgs) {
		
		try {
			return uploadImage(execArgs);
		} 
		catch (GingerException e) {
			Log.e("ImageUploadTask", e.toString());
		}
		
		return null;
	}
	
	protected ImageUploadResponse uploadImage(String... execArgs) throws GingerException {
		
		String serviceUrl = (execArgs != null && execArgs.length > 0) ? execArgs[0] : null;
		if(serviceUrl == null || serviceUrl.isEmpty()){
			String err = "ServiceUrl, first parameter of exec method, may not be null or empty";
			throw new GingerException(err);
		}
			    
	    InputStreamEntity reqEntity = null;
		try {
			File file = new File(this.request.getFullLocalPath());	
			reqEntity = new InputStreamEntity(new FileInputStream(file), -1);
		} 
		catch (FileNotFoundException e) {
			e.printStackTrace();
			String msg = "File not found: " + this.request.getFullLocalPath();
			throw new GingerException(msg, e);
		}
		
	    reqEntity.setContentType("binary/octet-stream");
	    reqEntity.setChunked(true); // Send in multiple parts if needed
	    
	    HttpPost httpPost = new HttpPost(serviceUrl);
	    httpPost.setEntity(reqEntity);
	    HttpResponse response = null;	    
	    try {
	    	HttpClient httpClient = GingerNetUtils.getDefaultHttpClientForFiles();
			response = httpClient.execute(httpPost);
		} 
	    catch (ClientProtocolException e) {
	    	e.printStackTrace();
	    	throw new GingerException("Protocol exception when uploading the file", e);
		} 
	    catch (IOException e) {
	    	e.printStackTrace();
	    	throw new GingerException("IO exception when uploading the file", e);
		}
	    
	    // below method throws GingerException
	    ImageUploadResponse result = GingerNetUtils.getJsonObjectFrom(
	    		response, ImageUploadResponse.class);
	    
		return result;
	}
	
    protected void onPostExecute(ImageUploadResponse response) {    	
    	super.onPostExecute(response);
    	this.listener.onUpload(this.request, response);
    }
	
	public interface ImageUploadListener{    	
    	public void onUpload(ImageUploadRequest request, ImageUploadResponse response);
    }
}

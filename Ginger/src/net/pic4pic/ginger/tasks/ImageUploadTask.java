package net.pic4pic.ginger.tasks;

import android.content.Context;

import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.ImageUploadRequest;
import net.pic4pic.ginger.entities.ImageUploadResponse;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.utils.MyLog;

public class ImageUploadTask extends BlockedTask<String, Void, ImageUploadResponse> {

	private ImageUploadRequest request;
	private ImageUploadListener listener;
	private boolean isSignUp;
	
	public ImageUploadTask(ImageUploadListener listener, Context context, ImageUploadRequest request, boolean isSignUp){
		
		super(context);
		this.listener = listener;
		this.request = request;
		this.isSignUp = isSignUp;
	}
	
	@Override
    protected ImageUploadResponse doInBackground(String... execArgs) {
		
		try {
			System.gc();
			return Service.getInstance().uploadImage(this.context, this.request);
		} 
		catch (GingerException e) {
			
			MyLog.bag().add(e).e();
			
			if(this.isSignUp){
				
				MyLog.bag()
				.add("funnel", "signup")
				.add("step", "5")
				.add("page", "uploadphoto")
				.add("action", "post upload")
				.add("success", "0")
				.add("error", "ginger")
				.m();	
			}
			
			
			ImageUploadResponse response = new ImageUploadResponse();
			response.setErrorCode(1);
			response.setErrorMessage(e.getMessage());
			return response;
		}
		catch(Throwable e){
			
			MyLog.bag().add(e).e();
			
			if(this.isSignUp){
			
				MyLog.bag()
				.add("funnel", "signup")
				.add("step", "5")
				.add("page", "uploadphoto")
				.add("action", "post upload")
				.add("success", "0")
				.add("error", "throwable")
				.m();
			}			
			
			ImageUploadResponse response = new ImageUploadResponse();
			response.setErrorCode(1);
			response.setErrorMessage("Connection to server failed when uploading picture. Please try again!");
			return response;	
		}
	}
		
    protected void onPostExecute(ImageUploadResponse response) {    	
    	super.onPostExecute(response);
    	this.listener.onUpload(this.request, response);
    }
	
	public interface ImageUploadListener{    	
    	public void onUpload(ImageUploadRequest request, ImageUploadResponse response);
    }
}

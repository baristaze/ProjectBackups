package net.pic4pic.ginger.service;

import java.util.UUID;

import android.content.Context;

import net.pic4pic.ginger.entities.BaseResponse;
import net.pic4pic.ginger.entities.FacebookRequest;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.ImageUploadRequest;
import net.pic4pic.ginger.entities.ImageUploadResponse;
import net.pic4pic.ginger.entities.SignupRequest;
import net.pic4pic.ginger.entities.UserCredentials;
import net.pic4pic.ginger.entities.UserResponse;
import net.pic4pic.ginger.entities.VerifyBioRequest;

public class ServiceProxy extends ServiceBase implements IService {

	@Override
	public UUID init(Context context){
		
		UUID clientId = super.getClientId(context);
		if(clientId == null){
			clientId = UUID.randomUUID();
			if(super.saveClientId(context, clientId)){
				return clientId;
			}
			else{
				return null;
			}
		}
		else{
			return clientId;
		}		
	}
	
	@Override
	public UserResponse checkUsername(Context context, UserCredentials request) throws GingerException {
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		UserResponse response = super.post(
				request, 
				UserResponse.class, 
				ServiceEndpoint.MainService, 
				null, 
				"/svc/rest/checkusername").getData();
		
		if(response.getAuthToken() != null && response.getAuthToken().trim().length() > 0){
			super.saveAuthToken(context, response.getAuthToken().trim());
		}
		
		return response;
	}

	@Override
	public UserResponse signin(Context context, UserCredentials request) throws GingerException {
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		UserResponse response = super.post(
				request, 
				UserResponse.class, 
				ServiceEndpoint.MainService, 
				null, 
				"/svc/rest/signin").getData();
		
		if(response.getAuthToken() != null && response.getAuthToken().trim().length() > 0){
			super.saveAuthToken(context, response.getAuthToken().trim());
		}
		
		return response;
	}

	@Override
	public UserResponse verifyBio(Context context, VerifyBioRequest request) throws GingerException {

		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				UserResponse.class, 
				ServiceEndpoint.MainService, 
				super.getAuthToken(context), 
				"/svc/rest/verifybio").getData();
	}
	
	@Override
	public BaseResponse downloadFriends(Context context, FacebookRequest request) throws GingerException{
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}		
		
		return super.post(
				request, 
				BaseResponse.class, 
				ServiceEndpoint.MainService, 
				super.getAuthToken(context), 
				"/svc/rest/friends").getData();	
	}

	@Override
	public UserResponse signup(Context context, SignupRequest request) throws GingerException {
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		UserResponse response = super.post(
				request, 
				UserResponse.class, 
				ServiceEndpoint.MainService, 
				null, 
				"/svc/rest/signup").getData();
		
		if(response.getAuthToken() != null && response.getAuthToken().trim().length() > 0){
			super.saveAuthToken(context, response.getAuthToken().trim());
		}
		
		return response;
	}
	
	@Override
	public ImageUploadResponse uploadProfileImage(Context context, ImageUploadRequest request) throws GingerException {
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.postLocalImage(
				request.getFullLocalPath(), 
				request.getClientId(),
				ImageUploadResponse.class, 
				ServiceEndpoint.ImageService, 
				super.getAuthToken(context, false), 
				"/file/upload?blurSize=20&profile=1&thumbx=200&thumby=200").getData();
	}
}

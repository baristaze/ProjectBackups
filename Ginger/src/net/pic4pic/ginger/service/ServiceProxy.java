package net.pic4pic.ginger.service;

import java.util.UUID;

import android.content.Context;

import net.pic4pic.ginger.entities.AcceptingPic4PicRequest;
import net.pic4pic.ginger.entities.BaseRequest;
import net.pic4pic.ginger.entities.BaseResponse;
import net.pic4pic.ginger.entities.FacebookRequest;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.ImageUploadRequest;
import net.pic4pic.ginger.entities.ImageUploadResponse;
import net.pic4pic.ginger.entities.MarkingRequest;
import net.pic4pic.ginger.entities.MatchedCandidateListResponse;
import net.pic4pic.ginger.entities.NotificationListResponse;
import net.pic4pic.ginger.entities.NotificationRequest;
import net.pic4pic.ginger.entities.Pic4PicHistory;
import net.pic4pic.ginger.entities.Pic4PicHistoryRequest;
import net.pic4pic.ginger.entities.SignupRequest;
import net.pic4pic.ginger.entities.SimpleResponseGuid;
import net.pic4pic.ginger.entities.StartingPic4PicRequest;
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
	
	@Override
	public MatchedCandidateListResponse getTodaysMatches(Context context, BaseRequest request) throws GingerException{
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				MatchedCandidateListResponse.class, 
				ServiceEndpoint.MainService, 
				super.getAuthToken(context), 
				"/svc/rest/matches").getData();
	}
	
	@Override
	public SimpleResponseGuid requestPic4Pic(Context context, StartingPic4PicRequest request) throws GingerException{
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				SimpleResponseGuid.class, 
				ServiceEndpoint.MainService, 
				super.getAuthToken(context), 
				"/svc/rest/p4p/request").getData();
	}
    
	@Override
    public SimpleResponseGuid acceptPic4Pic(Context context, AcceptingPic4PicRequest request) throws GingerException{
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				SimpleResponseGuid.class, 
				ServiceEndpoint.MainService, 
				super.getAuthToken(context), 
				"/svc/rest/p4p/accept").getData();	
    }
	
	@Override
	public Pic4PicHistory getPic4PicHistory(Context context, Pic4PicHistoryRequest request) throws GingerException{
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				Pic4PicHistory.class, 
				ServiceEndpoint.MainService, 
				super.getAuthToken(context), 
				"/svc/rest/p4p/history").getData();	
	}
	
	@Override
	public NotificationListResponse getNotifications(Context context, NotificationRequest request) throws GingerException{
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				NotificationListResponse.class, 
				ServiceEndpoint.MainService, 
				super.getAuthToken(context), 
				"/svc/rest/notifications").getData();
	}
	
	@Override
	public BaseResponse mark(Context context, MarkingRequest request) throws GingerException{
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				BaseResponse.class, 
				ServiceEndpoint.MainService, 
				super.getAuthToken(context), 
				"/svc/rest/mark").getData();	
	}
}

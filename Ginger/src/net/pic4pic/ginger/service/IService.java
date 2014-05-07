package net.pic4pic.ginger.service;

import java.util.UUID;

import net.pic4pic.ginger.entities.AcceptingPic4PicRequest;
import net.pic4pic.ginger.entities.BaseRequest;
import net.pic4pic.ginger.entities.BaseResponse;
import net.pic4pic.ginger.entities.FacebookRequest;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.ImageUploadRequest;
import net.pic4pic.ginger.entities.ImageUploadResponse;
import net.pic4pic.ginger.entities.MatchedCandidateListResponse;
import net.pic4pic.ginger.entities.NotificationListResponse;
import net.pic4pic.ginger.entities.NotificationRequest;
import net.pic4pic.ginger.entities.Pic4PicHistory;
import net.pic4pic.ginger.entities.Pic4PicHistoryRequest;
import net.pic4pic.ginger.entities.SimpleResponseGuid;
import net.pic4pic.ginger.entities.StartingPic4PicRequest;
import net.pic4pic.ginger.entities.UserResponse;
import net.pic4pic.ginger.entities.UserCredentials;
import net.pic4pic.ginger.entities.VerifyBioRequest;
import net.pic4pic.ginger.entities.SignupRequest;

import android.content.Context;

public interface IService {
	
    /**
     * ErrorCode != 0 : Username is already in use
     * ErrorCode == 0 & AuthToken == null => Username is available
     * ErrorCode == 0 & AuthToken != null => User is already signed up
     */
	
	public UUID init(Context context);
	
    public UserResponse checkUsername(Context context, UserCredentials request) throws GingerException;
    
    public UserResponse signin(Context context, UserCredentials request) throws GingerException;
    
    public UserResponse verifyBio(Context context, VerifyBioRequest request) throws GingerException;
    
    public UserResponse signup(Context context, SignupRequest request) throws GingerException; 
    
    public BaseResponse downloadFriends(Context context, FacebookRequest request) throws GingerException;
    
    public ImageUploadResponse uploadProfileImage(Context context, ImageUploadRequest request) throws GingerException;
    
    public MatchedCandidateListResponse getTodaysMatches(Context context, BaseRequest request) throws GingerException;
    
    public SimpleResponseGuid requestPic4Pic(Context context, StartingPic4PicRequest request) throws GingerException;
    
    public SimpleResponseGuid acceptPic4Pic(Context context, AcceptingPic4PicRequest request) throws GingerException;
    
    public Pic4PicHistory getPic4PicHistory(Context context, Pic4PicHistoryRequest request) throws GingerException;
    
    public NotificationListResponse getNotifications(Context context, NotificationRequest request) throws GingerException;
}

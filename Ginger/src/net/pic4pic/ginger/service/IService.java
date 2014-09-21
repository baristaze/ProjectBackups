package net.pic4pic.ginger.service;

import java.util.UUID;

import net.pic4pic.ginger.entities.AcceptingPic4PicRequest;
import net.pic4pic.ginger.entities.BaseRequest;
import net.pic4pic.ginger.entities.BaseResponse;
import net.pic4pic.ginger.entities.BuyingNewMatchRequest;
import net.pic4pic.ginger.entities.CandidateDetailsRequest;
import net.pic4pic.ginger.entities.CandidateDetailsResponse;
import net.pic4pic.ginger.entities.ClientLogRequest;
import net.pic4pic.ginger.entities.ConversationRequest;
import net.pic4pic.ginger.entities.ConversationResponse;
import net.pic4pic.ginger.entities.ConversationsSummaryResponse;
import net.pic4pic.ginger.entities.FacebookRequest;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.ImageUploadRequest;
import net.pic4pic.ginger.entities.ImageUploadResponse;
import net.pic4pic.ginger.entities.InstantMessageRequest;
import net.pic4pic.ginger.entities.Location;
import net.pic4pic.ginger.entities.MarkingRequest;
import net.pic4pic.ginger.entities.MatchedCandidateListResponse;
import net.pic4pic.ginger.entities.MatchedCandidateResponse;
import net.pic4pic.ginger.entities.MobileDevice;
import net.pic4pic.ginger.entities.NotificationListResponse;
import net.pic4pic.ginger.entities.NotificationRequest;
import net.pic4pic.ginger.entities.PurchaseOfferListResponse;
import net.pic4pic.ginger.entities.PurchaseRecord;
import net.pic4pic.ginger.entities.SimpleRequest;
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
	
	public UUID getCachedClientId();
	
	public int getLogReportingLevel();
	
    public UserResponse checkUsername(Context context, UserCredentials request) throws GingerException;
    
    public UserResponse signin(Context context, UserCredentials request) throws GingerException;
    
    public UserResponse verifyBio(Context context, VerifyBioRequest request) throws GingerException;
    
    public UserResponse signup(Context context, SignupRequest request) throws GingerException;
    
    public BaseResponse activateUser(Context context, BaseRequest request) throws GingerException;
    
    public BaseResponse updateUserDetails(Context context, SimpleRequest<String> request) throws GingerException;
    
    public BaseResponse downloadFriends(Context context, FacebookRequest request) throws GingerException;
    
    public ImageUploadResponse uploadImage(Context context, ImageUploadRequest request) throws GingerException;
    
    public MatchedCandidateListResponse getTodaysMatches(Context context, SimpleRequest<Location> request) throws GingerException;
    
    public MatchedCandidateListResponse getPreviewMatches(Context context, SimpleRequest<Location> request) throws GingerException;
    
    public MatchedCandidateListResponse buyNewMatches(Context context, BuyingNewMatchRequest request) throws GingerException;
    
    public MatchedCandidateResponse requestPic4Pic(Context context, StartingPic4PicRequest request) throws GingerException;
    
    public MatchedCandidateResponse acceptPic4Pic(Context context, AcceptingPic4PicRequest request) throws GingerException;
    
    public CandidateDetailsResponse getCandidateDetails(Context context, CandidateDetailsRequest request) throws GingerException;
    
    public NotificationListResponse getNotifications(Context context, NotificationRequest request) throws GingerException;
    
    public BaseResponse mark(Context context, MarkingRequest request) throws GingerException;
    
    public ConversationResponse sendInstantMessage(Context context, InstantMessageRequest request)  throws GingerException;
    
    public ConversationResponse getConversation(Context context, ConversationRequest request) throws GingerException;
    
    public ConversationsSummaryResponse getConversationSummary(Context context, BaseRequest request) throws GingerException;
    
    public BaseResponse processPurchase(Context context, SimpleRequest<PurchaseRecord> request) throws GingerException;
    
    public BaseResponse getCurrentCredit(Context context, BaseRequest request) throws GingerException;
    
    public PurchaseOfferListResponse getOffers(Context context, BaseRequest request) throws GingerException;
    
    public BaseResponse trackDevice(Context context, MobileDevice request) throws GingerException;
    
    public BaseResponse assureSupportAtLocation(Context context, SimpleRequest<Location> request) throws GingerException;
    
    public BaseResponse setCurrentLocation(Context context, SimpleRequest<Location> request) throws GingerException;
    
    public BaseResponse sendClientLogs(ClientLogRequest request) throws GingerException;
}

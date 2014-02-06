package net.pic4pic.ginger.service;

import java.util.UUID;

import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.ImageUploadRequest;
import net.pic4pic.ginger.entities.ImageUploadResponse;
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
    
    public ImageUploadResponse uploadProfileImage(Context context, ImageUploadRequest request) throws GingerException;
}

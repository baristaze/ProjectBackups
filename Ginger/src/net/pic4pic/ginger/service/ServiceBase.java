package net.pic4pic.ginger.service;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.util.UUID;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.entity.InputStreamEntity;

import android.content.Context;
import android.content.SharedPreferences;

import net.pic4pic.ginger.entities.BaseRequest;
import net.pic4pic.ginger.entities.BaseResponse;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.utils.GingerNetUtils;
import net.pic4pic.ginger.utils.MyLog;

public class ServiceBase {
	
	private String authTokenCached = null;
	
	protected UUID clientIdCached = null;
	
	/**
	 * Retrieves the authentication token either from cache or from local file
	 * @param context
	 * @return
	 */
	protected synchronized String getAuthToken(Context context) throws GingerException {
		return this.getAuthToken(context, true);
	}
	
	/**
	 * Retrieves the authentication token either from cache or from local file
	 * @param context
	 * @return
	 */
	protected synchronized String getAuthToken(Context context, boolean throwOnNoAuth) throws GingerException {
		
		if(this.authTokenCached == null){
			this.authTokenCached = this._getAuthTokenFromLocal(context);
			if(this.authTokenCached == null && throwOnNoAuth){
				throw new GingerException("Please sign in first");
			}
		}
		
		return this.authTokenCached;
	}
	
	/**
	 * Retrieves the client either from cache or from local file
	 * @param context
	 * @return
	 */
	protected synchronized UUID getClientId(Context context){
		
		if(this.clientIdCached == null){
			this.clientIdCached = this._getClientIdFromLocal(context);
		}
		
		return this.clientIdCached;
	}
	
	/**
	 * Saves the client ID to the local file and updates the cache.
	 * @param context
	 * @param clientId
	 * @return
	 */
	protected synchronized boolean saveClientId(Context context, UUID clientId){
		
		if(clientId != null){
			this._saveClientIdToLocal(context, clientId);
			this.clientIdCached = clientId;
			return true;
		}
		
		return false;
	}
	
	/**
	 * Saves the authentication token to the local file and updates the cache.
	 * @param context
	 * @param authToken
	 * @return
	 */
	protected synchronized boolean saveAuthToken(Context context, String authToken){
		
		if(authToken != null){
			this._saveAuthTokenToLocal(context, authToken);
			this.authTokenCached = authToken;
			return true;
		}
		
		return false;
	}
	
	/**
	 * Deletes the authentication token from the local file and clears the cache.
	 * @param context
	 */
	protected synchronized void clearAuthToken(Context context){
		
		if(this.authTokenCached != null){
			this._deleteAuthTokenFromLocal(context);
			this.authTokenCached = null;
		}
	}
	
	/**
	 * Retrieves the authentication token
	 * @param context
	 * @return
	 */
	private String _getAuthTokenFromLocal(Context context){
		
		// We have defined the key location in string.xml however there is no way to read it from here.
		// Therefore we will be using hard-coded value, which is fine.
		// ...
		// <string name="pref_filename_key">net.pic4pic.ginger.PREFERENCE_FILE</string>
		// <string name="pref_user_authtoken_key">saved_authtoken</string>
		// ...
		
		String fileKey = "net.pic4pic.ginger.PREFERENCE_FILE";
		String fieldKey = "saved_authtoken";
						
		SharedPreferences prefs = context.getSharedPreferences(fileKey, Context.MODE_PRIVATE);
		String authToken = prefs.getString(fieldKey, null);
		
		return authToken;
	}
	
	/**
	 * Saves the authentication token if it is not null
	 * @param context
	 * @param authToken
	 */
	private void _saveAuthTokenToLocal(Context context, String authToken){
		
		// We have defined the key location in string.xml however there is no way to read it from here.
		// Therefore we will be using hard-coded value, which is fine.
		// ...
		// <string name="pref_filename_key">net.pic4pic.ginger.PREFERENCE_FILE</string>
		// <string name="pref_user_authtoken_key">saved_authtoken</string>
		// ...
		
		String fileKey = "net.pic4pic.ginger.PREFERENCE_FILE";
		String fieldKey = "saved_authtoken";
		SharedPreferences prefs = context.getSharedPreferences(fileKey, Context.MODE_PRIVATE);
		
		SharedPreferences.Editor editor = prefs.edit();
		editor.putString(fieldKey, authToken);
		editor.commit();
	}
		
	/**
	 * Deletes the authentication token
	 * @param context
	 * @param authToken
	 */
	private void _deleteAuthTokenFromLocal(Context context){
		
		// We have defined the key location in string.xml however there is no way to read it from here.
		// Therefore we will be using hard-coded value, which is fine.
		// ...
		// <string name="pref_filename_key">net.pic4pic.ginger.PREFERENCE_FILE</string>
		// <string name="pref_user_authtoken_key">saved_authtoken</string>
		// ...
		
		String fileKey = "net.pic4pic.ginger.PREFERENCE_FILE";
		String fieldKey = "saved_authtoken";
		SharedPreferences prefs = context.getSharedPreferences(fileKey, Context.MODE_PRIVATE);
		
		SharedPreferences.Editor editor = prefs.edit();
		editor.remove(fieldKey);
		editor.commit();
	}
	
	/**
	 * Retrieves the authentication token
	 * @param context
	 * @return
	 */
	private UUID _getClientIdFromLocal(Context context){
		
		// We have defined the key location in string.xml however there is no way to read it from here.
		// Therefore we will be using hard-coded value, which is fine.
		// ...
		// <string name="pref_filename_key">net.pic4pic.ginger.PREFERENCE_FILE</string>
		// <string name="pref_user_clientid_key">saved_clientid</string>
		// ...
		
		String fileKey = "net.pic4pic.ginger.PREFERENCE_FILE";
		String fieldKey = "saved_clientid";
						
		SharedPreferences prefs = context.getSharedPreferences(fileKey, Context.MODE_PRIVATE);
		String clientIdText = prefs.getString(fieldKey, null);
		if(clientIdText != null && clientIdText.trim().length() > 0){
			return UUID.fromString(clientIdText);
		}
		
		return null;
	}
	
	/**
	 * Saves the authentication token if it is not null
	 * @param context
	 * @param authToken
	 */
	private void _saveClientIdToLocal(Context context, UUID clientId){
		
		// We have defined the key location in string.xml however there is no way to read it from here.
		// Therefore we will be using hard-coded value, which is fine.
		// ...
		// <string name="pref_filename_key">net.pic4pic.ginger.PREFERENCE_FILE</string>
		// <string name="pref_user_clientid_key">saved_clientid</string>
		// ...
		
		String fileKey = "net.pic4pic.ginger.PREFERENCE_FILE";
		String fieldKey = "saved_clientid";
		SharedPreferences prefs = context.getSharedPreferences(fileKey, Context.MODE_PRIVATE);
		
		SharedPreferences.Editor editor = prefs.edit();
		editor.putString(fieldKey, clientId.toString());
		editor.commit();
	}
	
	/**
	 * Makes an HTTP GET web service call. 
	 * The service should return a JSON response. 
	 * This method converts it to the specified object.  
	 * @param actionDescription
	 * @param outputClass
	 * @param webSvc
	 * @param authToken
	 * @param pathAndQuery
	 * @return
	 * @throws GingerException
	 */
	protected static<R extends BaseResponse> ResponseWrapper<R> get(
			String actionDescription, 
			Class<R> outputClass, 
			ServiceEndpoint webSvc, 
			String authToken, 
			String pathAndQuery) throws GingerException {
		
		// create post request
		String url = webSvc.getUrl(pathAndQuery);
		HttpGet httpGet = new HttpGet(url);
				
		// set authentication token
		if(authToken != null){
			httpGet.setHeader("XAuthToken", authToken);
		}
		
		// make the call in a safe way
		HttpResponse response = null;
		try {
			HttpClient httpClient = GingerNetUtils.getHttpClient();
			response = httpClient.execute(httpGet);
		} 
		catch (ClientProtocolException e) {
			e.printStackTrace();
			throw new GingerException("Protocol exception when " + actionDescription + " via remote server", e);
		} 
		catch (IOException e) {
			e.printStackTrace();
			throw new GingerException("IO exception when " + actionDescription + " via remote server", e);
		}
		
		// below method throws GingerException
		R result = GingerNetUtils.getJsonObjectFrom(response, outputClass);
		return new ResponseWrapper<R>(result);
	}
	
	/**
	 * Make a JSON POST web service call.
	 * The service should return a JSON response. 
	 * This method converts it to the specified object.
	 * @param input
	 * @param outputClass
	 * @param webSvc
	 * @param authToken
	 * @param pathAndQuery
	 * @return
	 * @throws GingerException
	 */
	protected static<T extends BaseRequest, R extends BaseResponse> ResponseWrapper<R> post(
			T input, 
			Class<R> outputClass, 
			ServiceEndpoint webSvc, 
			String authToken, 
			String pathAndQuery) throws GingerException {
						
		// set the input
		if(input == null){
			throw new GingerException("Input to post is null");
		}
		
		// below method converts the input to JSON and sets the content type to "application/json"
		HttpEntity inputEntity = GingerNetUtils.convertToHttpEntity(input);
		
		// create post request
		String url = webSvc.getUrl(pathAndQuery);	
		MyLog.bag().v("ServiceCall", url);
		HttpPost httpPost = new HttpPost(url);
		httpPost.setEntity(inputEntity);
		
		// set authentication token
		if(authToken != null){
			httpPost.setHeader("XAuthToken", authToken);
		}
		
		// make the call in a safe way
		HttpResponse response = null;
		try {
			HttpClient httpClient = GingerNetUtils.getHttpClient();
			response = httpClient.execute(httpPost);
		} 
		catch (ClientProtocolException e) {
			e.printStackTrace();
			throw new GingerException("Protocol exception when making a remote call to the web service", e);
		} 
		catch (IOException e) {
			e.printStackTrace();
			throw new GingerException("IO exception when when making a remote call to the web service", e);
		}
		
		// below method throws GingerException
		R result = GingerNetUtils.getJsonObjectFrom(response, outputClass);
		return new ResponseWrapper<R>(result);
	}
	
	/**
	 * Makes a file POST web service call.
	 * The service should return a JSON response. 
	 * This method converts it to the specified object.
	 * @param localImageFullPath
	 * @param outputClass
	 * @param webSvc
	 * @param authToken
	 * @param pathAndQuery
	 * @return
	 * @throws GingerException
	 */
	protected static<R extends BaseResponse> ResponseWrapper<R> postLocalImage(
			String localImageFullPath,
			UUID clientId,
			Class<R> outputClass, 
			ServiceEndpoint webSvc, 
			String authToken, 
			String pathAndQuery) throws GingerException {
		
		if(localImageFullPath == null || localImageFullPath.trim().length() == 0){
			throw new GingerException("Input image path is null or empty");
		}
		
		MyLog.bag().v("Service call", "File Path = " + localImageFullPath);
		
	    InputStreamEntity reqEntity = null;
		try {
			File file = new File(localImageFullPath);	
			reqEntity = new InputStreamEntity(new FileInputStream(file), -1);
		} 
		catch (FileNotFoundException e) {
			e.printStackTrace();
			String msg = "File not found: " + localImageFullPath;
			throw new GingerException(msg, e);
		}
		
		reqEntity.setContentType("binary/octet-stream");
	    reqEntity.setChunked(true); // Send in multiple parts if needed
	    
	    String url = webSvc.getUrl(pathAndQuery);
	    
	    MyLog.bag().v("Service call", "URL = " + url);
	    
	    HttpPost httpPost = new HttpPost(url);
	    httpPost.setEntity(reqEntity);
	    httpPost.addHeader("ClientId", clientId.toString());
	    HttpResponse response = null;	    
	    try {
	    	HttpClient httpClient = GingerNetUtils.getDefaultHttpClientForFiles();
			response = httpClient.execute(httpPost);
		} 
	    catch (ClientProtocolException e) {
			e.printStackTrace();
			throw new GingerException("Protocol exception when making a remote call to the web service", e);
		} 
		catch (IOException e) {
			e.printStackTrace();
			throw new GingerException("IO exception when when making a remote call to the web service", e);
		}
	    
	    // below method throws GingerException
	    R result = GingerNetUtils.getJsonObjectFrom(response, outputClass);
	    return new ResponseWrapper<R>(result);
	}
}

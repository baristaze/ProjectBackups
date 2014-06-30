package net.pic4pic.ginger.entities;

import java.io.Serializable;
import java.util.Date;

import net.pic4pic.ginger.utils.MyLog;

import org.json.JSONException;
import org.json.JSONObject;

import com.google.gson.annotations.SerializedName;

public class InAppPurchaseResult implements Serializable {
	
	private static final long serialVersionUID = 1;
	
	@SerializedName("orderId")
	protected String orderId;
	
	@SerializedName("packageName")
	protected String appPackageName;
	
	@SerializedName("productId")
	protected String productItemSku;
	
	@SerializedName("purchaseTime")
	protected Date purchaseTimeUTC;
	
	@SerializedName("purchaseState")
	protected InAppPurchaseState purchaseState;
	
	@SerializedName("developerPayload")
	protected String developerPayload;
	
	@SerializedName("purchaseToken")
	protected String purchaseToken;
	
	protected String originalData;
	protected String dataSignature;
	
	public static InAppPurchaseResult createFromJsonString(String purchaseData, String dataSignature) throws GingerException {
		
		try 
		{
			JSONObject jo = new JSONObject(purchaseData);
			
			InAppPurchaseResult result = new InAppPurchaseResult();
			
			result.originalData = purchaseData;
			result.dataSignature = dataSignature;
			
			result.orderId = jo.getString("orderId");
			result.appPackageName = jo.getString("packageName");
			result.productItemSku = jo.getString("productId");
			result.purchaseState = InAppPurchaseState.from(jo.getInt("purchaseState"));
			result.developerPayload = jo.getString("developerPayload");
			result.purchaseToken = jo.getString("purchaseToken");			
			// seconds are since 1970. compatible with Date(n). time is UTC
			result.purchaseTimeUTC = new Date(jo.getLong("purchaseTime"));
			return result;
		}
		catch (JSONException e) {
			String errMsg = "InApp purchase result has unexpected format";
			MyLog.e("InAppPurchaseResult", errMsg + ".\n" + purchaseData);
			throw new GingerException(errMsg, e);
		}	
	}
	
	/**
	 * @return the orderId
	 */
	public String getOrderId() {
		return orderId;
	}

	/**
	 * @param orderId the orderId to set
	 */
	public void setOrderId(String orderId) {
		this.orderId = orderId;
	}

	/**
	 * @return the appPackageName
	 */
	public String getAppPackageName() {
		return appPackageName;
	}

	/**
	 * @param appPackageName the appPackageName to set
	 */
	public void setAppPackageName(String appPackageName) {
		this.appPackageName = appPackageName;
	}

	/**
	 * @return the productItemSku
	 */
	public String getProductItemSku() {
		return productItemSku;
	}

	/**
	 * @param productItemSku the productItemSku to set
	 */
	public void setProductItemSku(String productItemSku) {
		this.productItemSku = productItemSku;
	}

	/**
	 * @return the purchaseTime
	 */
	public Date getPurchaseTimeUTC() {
		return purchaseTimeUTC;
	}

	/**
	 * @param purchaseTime the purchaseTime to set
	 */
	public void setPurchaseTimeUTC(Date purchaseTime) {
		this.purchaseTimeUTC = purchaseTime;
	}

	/**
	 * @return the purchaseState
	 */
	public InAppPurchaseState getPurchaseState() {
		return purchaseState;
	}

	/**
	 * @param purchaseState the purchaseState to set
	 */
	public void setPurchaseState(InAppPurchaseState purchaseState) {
		this.purchaseState = purchaseState;
	}

	/**
	 * @return the developerPayload
	 */
	public String getDeveloperPayload() {
		return developerPayload;
	}

	/**
	 * @param developerPayload the developerPayload to set
	 */
	public void setDeveloperPayload(String developerPayload) {
		this.developerPayload = developerPayload;
	}

	/**
	 * @return the purchaseToken
	 */
	public String getPurchaseToken() {
		return purchaseToken;
	}

	/**
	 * @param purchaseToken the purchaseToken to set
	 */
	public void setPurchaseToken(String purchaseToken) {
		this.purchaseToken = purchaseToken;
	}
	
	/**
	 * @return the originalData
	 */
	public String getOriginalData() {
		return originalData;
	}

	/**
	 * @param originalData the originalData to set
	 */
	public void setOriginalData(String originalData) {
		this.originalData = originalData;
	}

	/**
	 * @return the dataSignature
	 */
	public String getDataSignature() {
		return dataSignature;
	}

	/**
	 * @param dataSignature the dataSignature to set
	 */
	public void setDataSignature(String dataSignature) {
		this.dataSignature = dataSignature;
	}
}

package net.pic4pic.ginger.entities;

import java.io.Serializable;
import java.util.Date;

import com.google.gson.annotations.SerializedName;

public class PurchaseRecord implements Serializable {

	private static final long serialVersionUID = 1;
	
	/**
	 * Ginger specific ID
	 */
	@SerializedName("InternalOfferId")
	protected int internalOfferId;
	
	/**
	 * e.g. Google Play
	 */
	@SerializedName("AppStoreId")
	protected AppStoreType appStoreId;
	
	@SerializedName("PurchaseTimeUTC")
	protected Date purchaseTimeUTC;
	
	/**
	 * External ID
	 */
	@SerializedName("PurchaseInstanceId")
	protected String purchaseInstanceId;
	
	/**
	 * External reference
	 */
	@SerializedName("PurchaseReferenceToken")
	protected String purchaseReferenceToken;
	
	/**
	 * Ginger User state to increase security 
	 */
	@SerializedName("InternalPurchasePayLoad")
	protected String internalPurchasePayLoad;

	/**
	 * @return the internalOfferId
	 */
	public int getInternalOfferId() {
		return internalOfferId;
	}

	/**
	 * @param internalOfferId the internalOfferId to set
	 */
	public void setInternalOfferId(int internalOfferId) {
		this.internalOfferId = internalOfferId;
	}

	/**
	 * @return the appStoreId
	 */
	public AppStoreType getAppStoreId() {
		return appStoreId;
	}

	/**
	 * @param appStoreId the appStoreId to set
	 */
	public void setAppStoreId(AppStoreType appStoreId) {
		this.appStoreId = appStoreId;
	}

	/**
	 * @return the purchaseTimeUTC
	 */
	public Date getPurchaseTimeUTC() {
		return purchaseTimeUTC;
	}

	/**
	 * @param purchaseTimeUTC the purchaseTimeUTC to set
	 */
	public void setPurchaseTimeUTC(Date purchaseTimeUTC) {
		this.purchaseTimeUTC = purchaseTimeUTC;
	}

	/**
	 * @return the purchaseInstanceId
	 */
	public String getPurchaseInstanceId() {
		return purchaseInstanceId;
	}

	/**
	 * @param purchaseInstanceId the purchaseInstanceId to set
	 */
	public void setPurchaseInstanceId(String purchaseInstanceId) {
		this.purchaseInstanceId = purchaseInstanceId;
	}

	/**
	 * @return the purchaseReferenceToken
	 */
	public String getPurchaseReferenceToken() {
		return purchaseReferenceToken;
	}

	/**
	 * @param purchaseReferenceToken the purchaseReferenceToken to set
	 */
	public void setPurchaseReferenceToken(String purchaseReferenceToken) {
		this.purchaseReferenceToken = purchaseReferenceToken;
	}

	/**
	 * @return the internalPurchasePayLoad
	 */
	public String getInternalPurchasePayLoad() {
		return internalPurchasePayLoad;
	}

	/**
	 * @param internalPurchasePayLoad the internalPurchasePayLoad to set
	 */
	public void setInternalPurchasePayLoad(String internalPurchasePayLoad) {
		this.internalPurchasePayLoad = internalPurchasePayLoad;
	}
}

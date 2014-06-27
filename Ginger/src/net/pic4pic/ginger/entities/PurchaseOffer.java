package net.pic4pic.ginger.entities;

import java.io.Serializable;

import com.google.gson.annotations.SerializedName;

public class PurchaseOffer implements Serializable {
	
	private static final long serialVersionUID = 1;
	
	/**
	 * Ginger specific ID
	 */
	@SerializedName("InternalItemId")
	protected int internalItemId;
	
	/**
	 * Product SKU name
	 */
	@SerializedName("AppStoreId")
	protected AppStoreType appStoreId;
	
	@SerializedName("AppStoreItemId")
	protected String appStoreItemId;
	
	@SerializedName("InternalItemName")
	protected String internalItemName;
	
	@SerializedName("InternalItemDescription")
	protected String internalItemDescription;
	
	@SerializedName("ItemPriceInCents")
	protected int itemPriceInCents;
	
	@SerializedName("CreditValue")
	protected int creditValue;
	
	/**
	 * @return the itemId
	 */
	public int getInternalItemId() {
		return internalItemId;
	}

	/**
	 * @param internalItemId the internalItemId to set
	 */
	public void setInternalItemId(int internalItemId) {
		this.internalItemId = internalItemId;
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
	 * @return the internalItemName
	 */
	public String getInternalItemName() {
		return internalItemName;
	}

	/**
	 * @param internalItemName the internalItemName to set
	 */
	public void setInternalItemName(String internalItemName) {
		this.internalItemName = internalItemName;
	}

	public String getItemPrice(){
		double money = (double)this.getItemPriceInCents() / (double)100.0;
		return String.format("%4.2f", money);
	}
	
	/**
	 * @return the itemPriceInCents
	 */
	public int getItemPriceInCents() {
		return itemPriceInCents;
	}

	/**
	 * @param itemPriceInCents the itemPriceInCents to set
	 */
	public void setItemPriceInCents(int itemPriceInCents) {
		this.itemPriceInCents = itemPriceInCents;
	}

	/**
	 * @return the Product SKU name
	 */
	public String getAppStoreItemId() {
		return appStoreItemId;
	}

	/**
	 * @param appStoreItemId the appStoreItemId to set
	 */
	public void setAppStoreItemId(String appStoreItemId) {
		this.appStoreItemId = appStoreItemId;
	}

	/**
	 * @return the creditValue
	 */
	public int getCreditValue() {
		return creditValue;
	}

	/**
	 * @param creditValue the creditValue to set
	 */
	public void setCreditValue(int creditValue) {
		this.creditValue = creditValue;
	}
	
	/**
	 * @return the internalItemDescription
	 */
	public String getInternalItemDescription() {
		return internalItemDescription;
	}

	/**
	 * @param internalItemDescription the internalItemDescription to set
	 */
	public void setInternalItemDescription(String internalItemDescription) {
		this.internalItemDescription = internalItemDescription;
	}
}

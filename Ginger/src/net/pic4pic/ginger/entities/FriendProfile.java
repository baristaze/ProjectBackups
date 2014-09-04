package net.pic4pic.ginger.entities;

import com.google.gson.annotations.SerializedName;

public class FriendProfile extends UserProfile {
	
	private static final long serialVersionUID = 1;
	
	@SerializedName("Familiarity")
    protected Familiarity familiarity;

	/**
	 * @return the familiarity
	 */
	public Familiarity getFamiliarity() {
		return familiarity;
	}

	/**
	 * @param familiarity the familiarity to set
	 */
	public void setFamiliarity(Familiarity familiarity) {
		this.familiarity = familiarity;
	}
	
	@Override
	public String toString(){
		
		return this.getDisplayName() + " : " + this.getShortBio();
	}
}

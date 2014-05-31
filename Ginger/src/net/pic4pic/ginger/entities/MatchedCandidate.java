package net.pic4pic.ginger.entities;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.Date;
import java.util.UUID;

import com.google.gson.annotations.SerializedName;

public class MatchedCandidate implements Serializable {

	private static final long serialVersionUID = 1;
	
	@SerializedName("CandidateProfile")
    protected FriendProfile candidateProfile;
	
	@SerializedName("ProfilePics")
    protected PicturePair profilePics;

	@SerializedName("LastViewTimeUTC")
	protected Date lastViewTimeUTC;
	
	@SerializedName("LastLikeTimeUTC")
	protected Date lastLikeTimeUTC;

	@SerializedName("LastPendingPic4PicId")
	protected UUID lastPendingPic4PicId;
	
	@SerializedName("OtherPictures")
    protected ArrayList<PicturePair> otherPictures = new ArrayList<PicturePair>();
	
	/**
	 * shortcut to getCandidateProfile().getUserId()
	 * @return
	 */
	public UUID getUserId()	{
		return this.getCandidateProfile().getUserId();
	}
	
	/**
	 * @return the candidateProfile
	 */
	public FriendProfile getCandidateProfile() {
		return candidateProfile;
	}

	/**
	 * @param candidateProfile the candidateProfile to set
	 */
	public void setCandidateProfile(FriendProfile candidateProfile) {
		this.candidateProfile = candidateProfile;
	}

	/**
	 * @return the profilePics
	 */
	public PicturePair getProfilePics() {
		return profilePics;
	}

	/**
	 * @param profilePics the profilePics to set
	 */
	public void setProfilePics(PicturePair profilePics) {
		this.profilePics = profilePics;
	}

	/**
	 * @return the lastViewTimeUTC
	 */
	public Date getLastViewTimeUTC() {
		return lastViewTimeUTC;
	}

	/**
	 * @param lastViewTimeUTC the lastViewTimeUTC to set
	 */
	public void setLastViewTimeUTC(Date lastViewTimeUTC) {
		this.lastViewTimeUTC = lastViewTimeUTC;
	}
	
	/**
	 * Have I viewed this profile already?
	 * @return
	 */
	public boolean isViewed(){
		
		if(this.lastViewTimeUTC == null || this.lastViewTimeUTC.equals(new Date(0))){
			return false;
		}
		
		return true;
	}

	/**
	 * @return the lastLikeTimeUTC
	 */
	public Date getLastLikeTimeUTC() {
		return lastLikeTimeUTC;
	}

	/**
	 * @param lastLikeTimeUTC the lastLikeTimeUTC to set
	 */
	public void setLastLikeTimeUTC(Date lastLikeTimeUTC) {
		this.lastLikeTimeUTC = lastLikeTimeUTC;
	}
	
	/**
	 * Have I liked this profile already?
	 * @return
	 */
	public boolean isLiked(){
		
		if(this.lastLikeTimeUTC == null || this.lastLikeTimeUTC.equals(new Date(0))){
			return false;
		}
		
		return true;
	}
	
	/**
	 * @return the lastPendingPic4PicId
	 */
	public UUID getLastPendingPic4PicId() {
		return lastPendingPic4PicId;
	}

	/**
	 * Returns true if lastPendingPic4PicId has a valid value
	 * @return
	 */
	public boolean hasPic4PicPending(){
		
		if(this.lastPendingPic4PicId == null){
			return false;
		}
		
		if(this.lastPendingPic4PicId.equals(new UUID(0,0))){
			return false;
		}
		
		return true;
	}
	
	/**
	 * @param lastPendingPic4PicId the lastPendingPic4PicId to set
	 */
	public void setLastPendingPic4PicId(UUID lastPendingPic4PicId) {
		this.lastPendingPic4PicId = lastPendingPic4PicId;
	}
	
	/**
	 * @return the otherPictures
	 */
	public ArrayList<PicturePair> getOtherPictures() {
		return otherPictures;
	}
}

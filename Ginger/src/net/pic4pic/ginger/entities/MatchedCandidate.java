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
	
	@SerializedName("OtherPictures")
    protected ArrayList<PicturePair> otherPictures = new ArrayList<PicturePair>();
	
	@SerializedName("SentPic4PicsToCandidate")
	protected ArrayList<PicForPic> sentPic4PicsToCandidate = new ArrayList<PicForPic>();
	
	@SerializedName("SentPic4PicsByCandidate")
	protected ArrayList<PicForPic> sentPic4PicsByCandidate = new ArrayList<PicForPic>();
	
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
		//return lastPendingPic4PicId;
		return null;
	}
	
	/**
	 * @return the otherPictures
	 */
	public ArrayList<PicturePair> getOtherPictures() {
		return otherPictures;
	}
	
	/**
	 * @return the sent
	 */
	public ArrayList<PicForPic> getSentPic4PicsToCandidate() {
		return sentPic4PicsToCandidate;
	}

	/**
	 * @return the received
	 */
	public ArrayList<PicForPic> getSentPic4PicsByCandidate() {
		return sentPic4PicsByCandidate;
	}
	
	/**
	 * Gets familiarity
	 * @return familiarity
	 */
	public Familiarity getFamiliarity() {
		
        for(PicForPic p : this.sentPic4PicsToCandidate) {
            if (p.isAccepted()) {
                return Familiarity.Familiar;
            }
        }

        for(PicForPic p : this.sentPic4PicsByCandidate){
            if (p.isAccepted()) {
                return Familiarity.Familiar;
            }
        }

        return Familiarity.Stranger;
    }
	
	/**
	 * Gets last Pic4Pic request which is sent to me but hasn't been accepted by me
	 * @return
	 */
	public PicForPic getLastPendingPic4PicRequest(){
		
		for(PicForPic p : this.sentPic4PicsByCandidate){
            if (!p.isAccepted()) {
                return p;
            }
        }	
		
		return null;
	}
	
	public ArrayList<PicturePair> getNonTradedPicturesToBeUsedInPic4Pic(ArrayList<PicturePair> myOtherPictures){
		
		ArrayList<PicturePair> result = new ArrayList<PicturePair>();
		
		for(PicturePair pair : myOtherPictures){
						
			UUID imageGroupingId = pair.getGroupingImageId();
			
			boolean alreadySent = false;
			for(PicForPic sentP4P : this.sentPic4PicsToCandidate){
				if(imageGroupingId.equals(sentP4P.picId1) || imageGroupingId.equals(sentP4P.picId2)){
					alreadySent = true;
					break;
				}
			}
			
			if(alreadySent){
				continue;
			}
			
			boolean alreadyReceived = false;
			for(PicForPic receivedP4P : this.sentPic4PicsByCandidate){
				if(imageGroupingId.equals(receivedP4P.picId1) || imageGroupingId.equals(receivedP4P.picId2)){
					alreadyReceived = true;
					break;
				}
			}
			
			if(alreadyReceived){
				continue;
			}
			
			//
			result.add(pair);
		}
		
		return result;
	}
}

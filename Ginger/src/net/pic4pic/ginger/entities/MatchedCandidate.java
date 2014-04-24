package net.pic4pic.ginger.entities;

import java.io.Serializable;
import java.util.ArrayList;

import com.google.gson.annotations.SerializedName;

public class MatchedCandidate implements Serializable {

	private static final long serialVersionUID = 1;
	
	@SerializedName("CandidateProfile")
    protected FriendProfile candidateProfile;
	
	@SerializedName("ProfilePics")
    protected PicturePair profilePics;

	@SerializedName("OtherPictures")
    protected ArrayList<PicturePair> otherPictures = new ArrayList<PicturePair>();
    
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
	 * @return the otherPictures
	 */
	public ArrayList<PicturePair> getOtherPictures() {
		return otherPictures;
	}
}

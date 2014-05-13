package net.pic4pic.ginger.entities;

import java.util.ArrayList;

import com.google.gson.annotations.SerializedName;

public class CandidateDetailsResponse extends BaseResponse {

	private static final long serialVersionUID = 1;
	
	@SerializedName("Candidate")
	protected MatchedCandidate candidate;
	
	@SerializedName("SentPic4Pics")
	protected ArrayList<PicForPic> sentPic4Pics = new ArrayList<PicForPic>();
	
	@SerializedName("ReceivedPic4Pics")
	protected ArrayList<PicForPic> receivedPic4Pics = new ArrayList<PicForPic>();
	
	/**
	 * @return the candidate
	 */
	public MatchedCandidate getCandidate() {
		return candidate;
	}

	/**
	 * @param candidate the candidate to set
	 */
	public void setCandidate(MatchedCandidate candidate) {
		this.candidate = candidate;
	}

	/**
	 * @return the sent
	 */
	public ArrayList<PicForPic> getSentPic4Pics() {
		return sentPic4Pics;
	}

	/**
	 * @return the received
	 */
	public ArrayList<PicForPic> getReceivedPic4Pics() {
		return receivedPic4Pics;
	}
	
	/**
	 * Gets familiarity
	 * @return familiarity
	 */
	public Familiarity getFamiliarity() {
		
        for(PicForPic p : this.sentPic4Pics) {
            if (p.isAccepted()) {
                return Familiarity.Familiar;
            }
        }

        for(PicForPic p : this.receivedPic4Pics){
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
		
		for(PicForPic p : this.receivedPic4Pics){
            if (!p.isAccepted()) {
                return p;
            }
        }	
		
		return null;
	}
}

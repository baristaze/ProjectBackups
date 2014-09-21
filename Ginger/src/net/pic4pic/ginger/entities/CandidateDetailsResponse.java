package net.pic4pic.ginger.entities;

import java.util.ArrayList;
import java.util.UUID;

import com.google.gson.annotations.SerializedName;

public class CandidateDetailsResponse extends BaseResponse {

	private static final long serialVersionUID = 1;
	
	@SerializedName("Candidate")
	protected MatchedCandidate candidate;
	
	@SerializedName("SentPic4PicsToCandidate")
	protected ArrayList<PicForPic> sentPic4PicsToCandidate = new ArrayList<PicForPic>();
	
	@SerializedName("SentPic4PicsByCandidate")
	protected ArrayList<PicForPic> sentPic4PicsByCandidate = new ArrayList<PicForPic>();
	
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

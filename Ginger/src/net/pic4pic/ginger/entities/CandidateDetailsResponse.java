package net.pic4pic.ginger.entities;

import com.google.gson.annotations.SerializedName;

public class CandidateDetailsResponse extends BaseResponse {

	private static final long serialVersionUID = 1;
	
	@SerializedName("Candidate")
	protected MatchedCandidate candidate;
	
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
}

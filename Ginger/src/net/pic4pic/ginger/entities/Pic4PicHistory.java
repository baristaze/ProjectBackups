package net.pic4pic.ginger.entities;

import java.util.ArrayList;

import com.google.gson.annotations.SerializedName;

public class Pic4PicHistory extends BaseResponse {

	private static final long serialVersionUID = 1;
	
	@SerializedName("Sent")
	protected ArrayList<PicForPic> sent = new ArrayList<PicForPic>();
	
	@SerializedName("Received")
	protected ArrayList<PicForPic> received = new ArrayList<PicForPic>();
	
	/**
	 * @return the sent
	 */
	public ArrayList<PicForPic> getSent() {
		return sent;
	}

	/**
	 * @return the received
	 */
	public ArrayList<PicForPic> getReceived() {
		return received;
	}
	
	/**
	 * Gets familiarity
	 * @return familiarity
	 */
	public Familiarity getFamiliarity() {
		
        for(PicForPic p : this.sent) {
            if (p.isAccepted()) {
                return Familiarity.Familiar;
            }
        }

        for(PicForPic p : this.received){
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
		
		for(PicForPic p : this.received){
            if (!p.isAccepted()) {
                return p;
            }
        }	
		
		return null;
	}
}

package net.pic4pic.ginger.entities;

public enum InAppPurchaseState implements IntegerEnum {
	
    Purchased(0),
    Cancelled(1),
    Refunded(2);
    
    private final int value;
	
	private InAppPurchaseState(int value) {
		this.value = value;
	}

	public int getIntValue() {
		return this.value;
	}
	
	public static InAppPurchaseState from(int intVal) throws GingerException{
		
		if(intVal == Purchased.getIntValue()){
			return Purchased;
		}
		
		if(intVal == Cancelled.getIntValue()){
			return Cancelled;
		}
		
		if(intVal == Refunded.getIntValue()){
			return Refunded;
		}
		
		throw new GingerException("Unknown InAppPurchaseState enum: " + intVal);
	}
}


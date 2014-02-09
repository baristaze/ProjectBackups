package net.pic4pic.ginger.entities;

public enum MaritalStatus implements IntegerEnum {
    Unknown(0),
    Single(1),
    Married(2);
    
    private final int value;
	
	private MaritalStatus(int value) {
		this.value = value;
	}

	public int getIntValue() {
		return this.value;
	}
}

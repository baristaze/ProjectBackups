package net.pic4pic.ginger.entities;

public enum AssetState implements IntegerEnum {
    New(0),
    Disabled(1),
    Deleted(2);
    
    private final int value;
	
	private AssetState(int value) {
		this.value = value;
	}

	public int getIntValue() {
		return this.value;
	}
}

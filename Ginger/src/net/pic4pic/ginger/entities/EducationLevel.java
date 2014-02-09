package net.pic4pic.ginger.entities;

public enum EducationLevel implements IntegerEnum  {
    Unknown(0),
    Elementary(1),
    HighSchool(2),
    College(3),
    Master(4),
    PhdOrAbove(5);

    private final int value;
	
	private EducationLevel(int value) {
		this.value = value;
	}

	public int getIntValue() {
		return this.value;
	}
}

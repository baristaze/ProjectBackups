package net.pic4pic.ginger.entities;

import java.io.Serializable;
import java.util.UUID;
import java.util.Date;
import com.google.gson.annotations.SerializedName;

public class UserProfile implements Serializable {
	
	private static final long serialVersionUID = 1;
	
    @SerializedName("UserId")
    protected UUID userId;
    
    @SerializedName("IsActive")
    protected boolean isActive;
    
    @SerializedName("Username")
    protected String username;
    
    @SerializedName("DisplayName")
    protected String displayName;

    @SerializedName("Gender")
    protected Gender gender;

    @SerializedName("BirthDay")
    protected Date birthDay;
            
    @SerializedName("HometownCity")
    protected String hometownCity;

    @SerializedName("HometownState")
    protected String hometownState;

    @SerializedName("MaritalStatus")
    protected MaritalStatus maritalStatus;

    @SerializedName("Profession")
    protected String profession;

    @SerializedName("EducationLevel")
    protected EducationLevel educationLevel;

    @SerializedName("TimeZoneOffset")
    protected int timeZoneOffset;

    @SerializedName("CreateTimeUTC")
    protected Date createTimeUTC;

    @SerializedName("LastUpdateTimeUTC")
    protected Date lastUpdateTimeUTC;
    
    @SerializedName("Description")
    protected String description;

	@SerializedName("ShortBio")
    protected String shortBio;
    
	/**
	 * @return the userId
	 */
	public UUID getUserId() {
		return userId;
	}

	/**
	 * @param userId the userId to set
	 */
	public void setUserId(UUID userId) {
		this.userId = userId;
	}
	
	/**
	 * @return the isActive
	 */
	public boolean isActive() {
		return isActive;
	}

	/**
	 * @param isActive the isActive to set
	 */
	public void setActive(boolean isActive) {
		this.isActive = isActive;
	}

	/**
	 * @return the userName
	 */
	public String getUsername() {
		return username;
	}

	/**
	 * @param username the userName to set
	 */
	public void setUsername(String username) {
		this.username = username;
	}
	
	/**
	 * @return the displayName
	 */
	public String getDisplayName() {
		return displayName;
	}

	/**
	 * @param displayName the displayName to set
	 */
	public void setDisplayName(String displayName) {
		this.displayName = displayName;
	}

	/**
	 * @return the gender
	 */
	public Gender getGender() {
		return gender;
	}

	/**
	 * @param gender the gender to set
	 */
	public void setGender(Gender gender) {
		this.gender = gender;
	}

	/**
	 * @return the birthDay
	 */
	public Date getBirthDay() {
		return birthDay;
	}

	/**
	 * @param birthDay the birthDay to set
	 */
	public void setBirthDay(Date birthDay) {
		this.birthDay = birthDay;
	}

	/**
	 * @return the hometownCity
	 */
	public String getHometownCity() {
		return hometownCity;
	}

	/**
	 * @param hometownCity the hometownCity to set
	 */
	public void setHometownCity(String hometownCity) {
		this.hometownCity = hometownCity;
	}

	/**
	 * @return the hometownState
	 */
	public String getHometownState() {
		return hometownState;
	}

	/**
	 * @param hometownState the hometownState to set
	 */
	public void setHometownState(String hometownState) {
		this.hometownState = hometownState;
	}

	/**
	 * @return the maritalStatus
	 */
	public MaritalStatus getMaritalStatus() {
		return maritalStatus;
	}

	/**
	 * @param maritalStatus the maritalStatus to set
	 */
	public void setMaritalStatus(MaritalStatus maritalStatus) {
		this.maritalStatus = maritalStatus;
	}

	/**
	 * @return the profession
	 */
	public String getProfession() {
		return profession;
	}

	/**
	 * @param profession the profession to set
	 */
	public void setProfession(String profession) {
		this.profession = profession;
	}

	/**
	 * @return the educationLevel
	 */
	public EducationLevel getEducationLevel() {
		return educationLevel;
	}

	/**
	 * @param educationLevel the educationLevel to set
	 */
	public void setEducationLevel(EducationLevel educationLevel) {
		this.educationLevel = educationLevel;
	}

	/**
	 * @return the timeZoneOffset
	 */
	public int getTimeZoneOffset() {
		return timeZoneOffset;
	}

	/**
	 * @param timeZoneOffset the timeZoneOffset to set
	 */
	public void setTimeZoneOffset(int timeZoneOffset) {
		this.timeZoneOffset = timeZoneOffset;
	}

	/**
	 * @return the createTimeUTC
	 */
	public Date getCreateTimeUTC() {
		return createTimeUTC;
	}

	/**
	 * @param createTimeUTC the createTimeUTC to set
	 */
	public void setCreateTimeUTC(Date createTimeUTC) {
		this.createTimeUTC = createTimeUTC;
	}

	/**
	 * @return the lastUpdateTimeUTC
	 */
	public Date getLastUpdateTimeUTC() {
		return lastUpdateTimeUTC;
	}

	/**
	 * @param lastUpdateTimeUTC the lastUpdateTimeUTC to set
	 */
	public void setLastUpdateTimeUTC(Date lastUpdateTimeUTC) {
		this.lastUpdateTimeUTC = lastUpdateTimeUTC;
	}
	
	/**
	 * @return the description
	 */
	public String getDescription() {
		return description;
	}

	/**
	 * @param description the description to set
	 */
	public void setDescription(String description) {
		this.description = description;
	}
	
	/**
	 * @return the shortBio
	 */
	public String getShortBio() {
		return shortBio;
	}

	/**
	 * @param shortBio the shortBio to set
	 */
	public void setShortBio(String shortBio) {
		this.shortBio = shortBio;
	}

	/**
	 * gets the education level as string
	 * @return
	 */
	public String getEducationLevelAsString(){
		
		if(this.educationLevel == EducationLevel.Elementary){
			return "Elementary";
		}
		else if(this.educationLevel == EducationLevel.HighSchool){
			return "High School";
		}
		else if(this.educationLevel == EducationLevel.College){
			return "College";
		}		
		else if(this.educationLevel == EducationLevel.Master){
			return "Master";
		}
		else if(this.educationLevel == EducationLevel.PhdOrAbove){
			return "PhD or above";
		}
		
	    return "Unknown";	    
	}
}

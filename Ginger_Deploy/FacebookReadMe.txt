See:
https://developers.facebook.com/apps/498161566945981/settings/


Instead of below methods, see App Logs. We print the hash-key

---------------------------------------
---------------------------------------
DEBUG
---------------------------------------
To Generate App's HashKey for Facebook:
keytool -exportcert -alias androiddebugkey -keystore %HOMEPATH%\.android\debug.keystore | openssl sha1 -binary | openssl base64

Result:
RMrc8BW2IWzKLLDJqGhtAXRbiqQ=


---------------------------------------
---------------------------------------
RELEASE
---------------------------------------
To Generate App's HashKey for Facebook:
keytool -exportcert -alias androidreleasekey1 -keystore D:\_CloudDrive\GitHubRepoRoot\PrivateRepo1\Ginger_Deploy\KeyStore\release_01.keystore | openssl sha1 -binary | openssl base64

Result:
yOpkFXMh75nkYzbx25YD78lesQ4=

Hint: alias is also important
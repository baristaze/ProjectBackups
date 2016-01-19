set java="C:\Program Files (x86)\Java\jre7\bin\java.exe"
set jmin="D:\_CloudDrive\Crosspl\Buildx\yuicompressor-2.4.7.jar"
"C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\tf.exe" checkout /lock:none "D:\_CloudDrive\Crosspl\Crosspl.Web\Scripts\js-bundle-min.js"
pushd "D:\_CloudDrive\Crosspl\Crosspl.Web\Scripts"
for /r %%i in (*.debug.js) do (%java% -jar %jmin% %%i -o ".\mini\%%~ni.mini.js")
REM options that might go to at the end of above command before ')' : --nomunge --preserve-semi --disable-optimizations
copy /Y /b ".\mini\*.js" ".\js-bundle-min.js"
del /Q .\mini\*
popd
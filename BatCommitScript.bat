cd %cd%
git status
pause
git add .
git status 
pause
SET stamp=%RANDOM%BatchCommit
git commit -m %stamp%
git push
pause

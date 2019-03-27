cd %cd%
git status
pause
git add .
SET stamp=%RANDOM%BatchCommit
git commit -m %stamp%
git push
pause

@echo off

set TEMPFILE="%TEMP%\BIZBUILD-SVN-%RANDOM%-%RANDOM%-%RANDOM%-%RANDOM%"
set SVNREV="%~1\svnrev.cs"

rem try generating svnrev from svn now. this will fail if svn is nonexistent, so...
"%~1\SubWCRev.exe" "%~1\..\." "%~1\svnrev_template" %TEMPFILE% > NUL

rem generate a svnrev with sed using no revision number, in case svn isnt available
if not exist %TEMPFILE% (
    "%~1\sed.exe" s/\$WCREV\$/0/ < "%~1\svnrev_template" > %TEMPFILE%
)

rem ... ignore the error
SET ERRORLEVEL=0


rem if we didnt even have a svnrev, then go ahead and copy it
if not exist %SVNREV% (
   copy /y %TEMPFILE% %SVNREV% > NUL
) else if exist %TEMPFILE% (
  rem check to see whether its any different, so we dont touch unchanged files
  fc /b %TEMPFILE% %SVNREV% > NUL
  if ERRORLEVEL 2 (
    echo Updated svnrev file
    copy /y %TEMPFILE% %SVNREV% > NUL
    goto SKIP
  )
  if ERRORLEVEL 1 (
    echo Updated svnrev file
    copy /y %TEMPFILE% %SVNREV% > NUL
    goto SKIP
  )
  if ERRORLEVEL 0 (
    echo svnrev is indicating no changes
    goto SKIP
  )  


) else (
  echo Ran into a weird error writing subwcrev output to tempfile: %TEMPFILE%
)

:SKIP

rem <zero> make subwcrev process more reliable. sorry for leaving so many tempfiles, but life's too short
rem del %TEMPFILE%

rem always let build proceed
SET ERRORLEVEL=0
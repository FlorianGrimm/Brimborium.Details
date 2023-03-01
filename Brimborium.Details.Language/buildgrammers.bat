@REM %USERPROFILE%\AppData\Roaming\Python\Python39\Scripts/antlr4.exe DetailsLanguage.g4 -o Generated -Dlanguage=CSharp
@REM %USERPROFILE%\AppData\Roaming\Python\Python39\Scripts/antlr4.exe MarkdownLanguage.g4 -o Generated -Dlanguage=CSharp
@REM %USERPROFILE%\AppData\Roaming\Python\Python39\Scripts/antlr4.exe CSharpLanguage.g4 -o Generated -Dlanguage=CSharp

java -jar .\antlr-4.12.0-complete.jar DetailsLanguage.g4 -o Generated -Dlanguage=CSharp -listener -visitor
java -jar .\antlr-4.12.0-complete.jar MarkdownLanguage.g4 -o Generated -Dlanguage=CSharp -listener -visitor
java -jar .\antlr-4.12.0-complete.jar CSharpLanguage.g4 -o Generated -Dlanguage=CSharp -listener -visitor
cls
dotnet run
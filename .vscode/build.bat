echo off

REM アセンブラり削除
DEL .\1.4\Assemblies\*.*

REM ビルド
dotnet build .\Source\LegacyFairy_Race_1.4\LegacyFairy_Race
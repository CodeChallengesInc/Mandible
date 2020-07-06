using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallengeInc.SubmissionApi.Constants
{
	public class FileInformation
	{
		public static string SubmissionFolder = "Submissions";
		public static string BackupSubmissionFolder = "Backups";
		public static string DateTimeFormat = "yyyy.MM.dd - H.mm.ss";

		// LoneAnt specific properties
		public static string LoneAntFileExtension = ".js";
		public static string DockerFilePathPrefix = "Submissions/";
		public static string DefaultAntString = @"/*
Example outputs:
    {cell:0}: move to cell 0
    {cell:4}: move to cell 4 (that is, do nothing, as 4 is the central cell)
    {cell:4, color:8}: set own cell to color 8
    {cell:6, color:1}: set cell 6 to color 1
    {cell:6, color:0}: equivalent to just `{cell:6}` - move rather than set color

Invalid Outputs:
    {cell: 9}: cell must be between 0-8.
    {cell0, color:9}: color must be from 1-8.

antView contains the cells around the ant.  Each item in the array contains:
    color: a number from 1-8
    food: 1 if present, 0 if not.

Cells are ordered in the following way:
        0 1 2
        3 4 5
        6 7 8

Note: Your orientation will be randomized
*/
var antView = view;

return {
    cell: 1
};
";
	}
}

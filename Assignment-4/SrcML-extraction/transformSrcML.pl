# This script is modified version of transforUnderstand.pl provided in the EECS 4314 lab
# It's used to transform the dependancy csv data into TA format
# The difference between the two, is that current script considers only first 2 columns of the CSV file
#
#/usr/bin/perl
# 
use strict;
use warnings;

my ($header, $from, $to, $output_file);
my (%FILE_HASH, %CALL_HASH, $name, $value, $count, $hash, $line);

$header = 0;

open INPUT, "$ARGV[0]";

if ($ARGV[0] =~ /(.*)\.csv/) {
	$output_file = $1 . ".raw.ta";
}

open OUTPUT, ">$output_file";

print OUTPUT "FACT TUPLE :\n";


while (<INPUT>) {
	$line =$_;
	chomp $line;

	if ($header == 0) {
		$header++;
		next;
	}

	($from, $to) = split(/,/, $line);

	# Remove newline characters from file names
	$from =~ s/\R//g;
	$to =~ s/\R//g;

	$FILE_HASH{$from}++;
	$FILE_HASH{$to}++;
	$CALL_HASH{$from}{$to}++;
}

# output the list of files
while (($name, $value) = (each %FILE_HASH)) {
	$name =~ s/\"//g;
	print OUTPUT "\$INSTANCE $name cFile\n";
}

# output the list of call relations
while (($name, $hash) = (each %CALL_HASH)) {
	while (($value, $count) = (each %$hash)) {
		$name =~ s/\"//g;
		$value =~ s/\"//g;
		print OUTPUT "cLinks $name $value\n";
	}
}

close INPUT;
close OUTPUT;

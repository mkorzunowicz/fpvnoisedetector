#!/bin/bash

# Check if a semantic version was provided as a parameter
if [ $# -eq 0 ]
  then
    echo "No semantic version provided"
    exit 1
fi

# Create URL with the provided version
REPO="https://github.com/mkorzunowicz/fpvnoisedetector"
URL="$REPO/releases/download/$1/fpvnoise-$1.zip"
CHANGELOG="$REPO/releases/tag/$1"
# Create update.xml file
cat > update.xml <<EOF
<?xml version="1.0" encoding="UTF-8"?>
<item>
  <version>$1</version>
  <url>$URL</url>
  <changelog>$CHANGELOG</changelog>
  <mandatory>false</mandatory>
</item>
EOF

echo "update.xml file created with version $1"
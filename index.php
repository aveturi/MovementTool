<?php


if(strlen($_POST["name"])){
$file = "movements/".$_POST["name"];
$fp = fopen($file, "w") or die("Couldn't open $file for writing!");
fwrite($fp, $_POST["movement"]) or die("Couldn't write values to file!"); 

fclose($fp); 
echo "Saved to $file successfully!";
}
$req = $_GET["name"];
$data = file_get_contents("movements/".$req);
echo $data;
?>
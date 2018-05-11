package main

import (
	"log"
	"net/http"
)

func main() {
	// Simple static webserver:
	log.Fatal(http.ListenAndServe(":8345", http.FileServer(http.Dir("../Release/"))))
}

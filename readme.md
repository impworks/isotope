# Isotope

A lightweight photo gallery.

## Features

* Adaptive UI: looks and feels great on both desktop and mobile
* Folders and tags support
* Search by tags and date ranges
* Access restriction: make your gallery available to anyone, or require user authorization
* Public links: share a particular folder or precise search results without revealing anything else

## Screenshots

#### Front-end:

<a href="https://user-images.githubusercontent.com/604496/102710904-e05bc300-42c6-11eb-8286-93f9581ed716.png"><img src="https://user-images.githubusercontent.com/604496/102710922-0a14ea00-42c7-11eb-90df-e4c004c8b2d1.png" /></a> <a href="https://user-images.githubusercontent.com/604496/102710929-1731d900-42c7-11eb-8267-1866540e987b.png"><img src="https://user-images.githubusercontent.com/604496/102710940-26188b80-42c7-11eb-9eb0-c1bba55dcbb9.png" /></a> <a href="https://user-images.githubusercontent.com/604496/102710947-33ce1100-42c7-11eb-8e3a-33c49a845bf9.png"><img src="https://user-images.githubusercontent.com/604496/102710953-3df00f80-42c7-11eb-8f54-e00ead673e31.png" /></a>

#### Management area:

<a href="https://github.com/user-attachments/assets/8535408c-4926-4206-9209-5093b5c24f40"><img width="250" height="177" alt="Folders view" src="https://github.com/user-attachments/assets/14719e7a-a2b9-4a1d-a1e3-02dc3da0dee4" /></a> <a href="https://github.com/user-attachments/assets/106e4551-2952-4d34-93e5-12545ebe52c0"><img width="250" height="177" alt="Media tagging" src="https://github.com/user-attachments/assets/d231751d-742f-478e-8241-d9d2b8e10ad8" /></a> <a href="https://github.com/user-attachments/assets/8b58ce2c-46c5-4bf7-a2d0-bf7afeb6db9b"><img width="250" height="177" alt="Properties editor" src="https://github.com/user-attachments/assets/0fefaa87-fdfb-4a92-8ba2-a91ab813df14" /></a> <a href="https://github.com/user-attachments/assets/02cb24bc-0ad5-4843-a205-6d731d2a3677"><img width="250" height="177" alt="Uploading" src="https://github.com/user-attachments/assets/7ecb07ca-0f25-4431-b745-3da68f9434eb" /></a>

## Installation

1. Pull the container from Docker:

    ```
    docker pull impworks/isotope:latest
    ```

2. Map container port `5000` to whatever port you want on the outside (e.g. 8080)
3. Map container path `/app/App_Data` to an external folder where you want to store your photos and database
4. Open the web UI and log in as `admin@example.com` using password `123456`
5. Create your own user instead and remove the default one

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

<a href="https://user-images.githubusercontent.com/604496/102710966-5bbd7480-42c7-11eb-9691-1f9cf7bf89fa.png"><img src="https://user-images.githubusercontent.com/604496/102710973-6415af80-42c7-11eb-92fe-b83b1ac36963.png" /></a> <a href="https://user-images.githubusercontent.com/604496/102710974-68da6380-42c7-11eb-9396-9b61048c0313.png"><img src="https://user-images.githubusercontent.com/604496/102710982-755ebc00-42c7-11eb-85e8-58d2a5324081.png" /></a> <a href="https://user-images.githubusercontent.com/604496/102710987-7a237000-42c7-11eb-854f-8d3aa831e39a.png"><img src="https://user-images.githubusercontent.com/604496/102710994-83144180-42c7-11eb-9ec6-1a13126af291.png" /></a> <a href="https://user-images.githubusercontent.com/604496/102710996-860f3200-42c7-11eb-9a7d-b8d002416aa7.png"><img src="https://user-images.githubusercontent.com/604496/102711002-90313080-42c7-11eb-955c-6e7723824078.png" /></a>

## Installation

Pull the container from Docker:

```
docker pull impworks/isotope:latest
```

## Backups

You need to mount the following folders to the container for backups:

`/app/Storage` - database and encryption keys (about 1MB)
`/app/wwwroot/@media` - actual media (size may vary)
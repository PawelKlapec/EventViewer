#!/bin/bash

docker container inspect eventViewerStandalone > /dev/null 2>&1 && \
(echo -n 'Stopping container: ' && docker container stop eventViewerStandalone) && \
(echo -n 'Removing container: ' && docker container rm  eventViewerStandalone)
docker run --name eventViewerStandalone --network host event_viewer $1

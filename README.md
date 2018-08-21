# Dash It All!

## What is Dash It All?

This is a C# .NET Core app that lets your sniff network packets and trigger web requests based on configurable rules.

## Wait, but why?

This past Amazon Prime Day, I bought 20 Dash Buttons for $1 each with the intent to turn them into smart buttons so I can hijack them to turn on my AC, record work hours, and basically do whatever I want. I liked Ted Benson's [article on hacking the Amazon Dash Button](https://blog.cloudstitch.com/how-i-hacked-amazon-s-5-wifi-button-to-track-baby-data-794214b0bdd8), but I don't want to install Python, and I want to add a management layer and share it with the world. So this project was born.

This is currently running on my Raspberry Pi at home and works well.

## Usage

### 1. Configure

Execute the `configure` command to create an example json file:

```sh
dash-it-all configure -c '<a-file-path>.json'
```

This will prompt you to select a capture device and writes the config json to the path specified.

### 2. Discover

Execute the `discover` command to see the devices active on the network:

```sh
dash-it-all discover -c '<a-file-path>.json'
```

This will list the MAC address of each device as it joins the network. Press your Dash button while running this command to get its MAC Address. Then you can update the json file to add a trigger for it.

### 3. Run

Execute the `run` command to start capturing packets on the network and processing the rules confiured:

```sh
dash-it-all run -c '<a-file-path>.json'
```

This will listen for packets as configured in the configuration file and trigger the specified web actions.
import React from 'react';
import { Link } from 'react-router-dom';
import {
  AppBar,
  Toolbar,
  Typography,
  IconButton,
  Drawer,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  Divider,
  CssBaseline,
  Box,
  Container,
  Grid,
  Paper,
  Button,
} from '@mui/material';
import {
  Menu as MenuIcon,
  Assignment as AssignmentIcon,
  Timer as TimerIcon,
  BarChart as BarChartIcon,
  TipsAndUpdates as TipsAndUpdatesIcon,
} from '@mui/icons-material';
import { Line } from 'react-chartjs-2';
import { useTheme } from '@mui/material/styles';

// Chart.js reg
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title as ChartTitle,
  Tooltip,
  Legend,
} from 'chart.js';

// reg Chart.js components
ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  ChartTitle,
  Tooltip,
  Legend
);

const drawerWidth = 240;

const Dashboard = () => {
  const theme = useTheme();
  const [mobileOpen, setMobileOpen] = React.useState(false);

  const chartData = {
    labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
    datasets: [
      {
        label: 'Hours Studied',
        data: [2, 3, 1, 4, 5, 2, 3],
        backgroundColor: theme.palette.primary.light,
        borderColor: theme.palette.primary.main,
        borderWidth: 2,
        fill: true,
      },
    ],
  };

  const chartOptions = {
    responsive: true,
    plugins: {
      legend: {
        position: 'top',
        labels: {
          color: theme.palette.text.primary,
        },
      },
      title: {
        display: true,
        text: 'Productivity Over the Week',
        color: theme.palette.text.primary,
      },
    },
    scales: {
      x: {
        ticks: { color: theme.palette.text.primary },
      },
      y: {
        ticks: { color: theme.palette.text.primary },
      },
    },
  };

  const handleDrawerToggle = () => {
    setMobileOpen(!mobileOpen);
  };

  const drawer = (
    <div>
      <Toolbar />
      <Divider />
      <List>
        <ListItem button>
          <ListItemIcon>
            <AssignmentIcon />
          </ListItemIcon>
          <ListItemText primary="Today's Tasks" />
        </ListItem>
        <ListItem button>
          <ListItemIcon>
            <TimerIcon />
          </ListItemIcon>
          <ListItemText primary="Study Timer" />
        </ListItem>
        <ListItem button>
          <ListItemIcon>
            <BarChartIcon />
          </ListItemIcon>
          <ListItemText primary="Productivity Stats" />
        </ListItem>
        <ListItem button component={Link} to="/studyzone">
          <ListItemIcon>
            <TipsAndUpdatesIcon />
          </ListItemIcon>
          <ListItemText primary="Study Zone" />
        </ListItem>
      </List>
    </div>
  );

  return (
    <Box sx={{ display: 'flex' }}>
      <CssBaseline />
      {/* AppBar */}
      <AppBar
        position="fixed"
        sx={{
          zIndex: theme.zIndex.drawer + 1,
          width: { sm: `calc(100% - ${drawerWidth}px)` },
          ml: { sm: `${drawerWidth}px` },
          backgroundColor: theme.palette.primary.main,
        }}
      >
        <Toolbar>
          {/* Menu Icon for mobile */}
          <IconButton
            color="inherit"
            aria-label="open drawer"
            edge="start"
            onClick={handleDrawerToggle}
            sx={{ mr: 2, display: { sm: 'none' } }}
          >
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" noWrap sx={{ flexGrow: 1 }}>
            Study Dashboard
          </Typography>
          {/*Study Zone Button */}
          <Button
            color="inherit"
            component={Link}
            to="/studyzone"
            sx={{ textTransform: 'none' }}
          >
            Study Zone
          </Button>
        </Toolbar>
      </AppBar>
      {/* Drawer */}
      <Box
        component="nav"
        sx={{ width: { sm: drawerWidth }, flexShrink: { sm: 0 } }}
        aria-label="navigation folders"
      >
        {/* Mobile Drawer */}
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onClose={handleDrawerToggle}
          ModalProps={{ keepMounted: true }}
          sx={{
            display: { xs: 'block', sm: 'none' },
            '& .MuiDrawer-paper': { width: drawerWidth },
          }}
        >
          {drawer}
        </Drawer>
        {/* Desktop Drawer */}
        <Drawer
          variant="permanent"
          sx={{
            display: { xs: 'none', sm: 'block' },
            '& .MuiDrawer-paper': {
              width: drawerWidth,
              boxSizing: 'border-box',
              backgroundColor: theme.palette.primary.main,
              color: '#fff',
            },
          }}
          open
        >
          {drawer}
        </Drawer>
      </Box>
      {/* Main Content */}
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: 3,
          backgroundColor: theme.palette.background.default,
          minHeight: '100vh',
        }}
      >
        <Toolbar />
        <Container maxWidth="lg">
          <Typography variant="h4" gutterBottom>
            Welcome Back!
          </Typography>
          <Grid container spacing={3}>
            {/* Today's Tasks */}
            <Grid item xs={12} md={4}>
              <Paper elevation={3} sx={{ p: 2 }}>
                <Typography variant="h6" gutterBottom>
                  Today's Tasks
                </Typography>
                <List>
                  <ListItem>
                    <ListItemIcon>
                      <AssignmentIcon color="primary" />
                    </ListItemIcon>
                    <ListItemText primary="Review lecture notes" />
                  </ListItem>
                  <ListItem>
                    <ListItemIcon>
                      <AssignmentIcon color="primary" />
                    </ListItemIcon>
                    <ListItemText primary="Complete assignment" />
                  </ListItem>
                  <ListItem>
                    <ListItemIcon>
                      <AssignmentIcon color="primary" />
                    </ListItemIcon>
                    <ListItemText primary="Practice problems" />
                  </ListItem>
                </List>
                <Button variant="contained" color="primary" fullWidth sx={{ mt: 2 }}>
                  Add New Task
                </Button>
              </Paper>
            </Grid>
            {/* Study Timer */}
            <Grid item xs={12} md={4}>
              <Paper elevation={3} sx={{ p: 2, textAlign: 'center' }}>
                <Typography variant="h6" gutterBottom>
                  Study Timer
                </Typography>
                <Typography variant="h3" color="primary" sx={{ my: 2 }}>
                  00:00:00
                </Typography>
                <Button variant="contained" color="secondary" sx={{ mr: 1 }}>
                  Start
                </Button>
                <Button variant="outlined" color="secondary" sx={{ ml: 1 }}>
                  Reset
                </Button>
              </Paper>
            </Grid>
            {/* Productivity Stats */}
            <Grid item xs={12} md={4}>
              <Paper elevation={3} sx={{ p: 2 }}>
                <Typography variant="h6" gutterBottom>
                  Productivity Stats
                </Typography>
                <Line data={chartData} options={chartOptions} />
              </Paper>
            </Grid>
          </Grid>
          {/*Study Tips */}
          <Grid container spacing={3} sx={{ mt: 2 }}>
            <Grid item xs={12}>
              <Paper elevation={3} sx={{ p: 2 }}>
                <Typography variant="h6" gutterBottom>
                  Personalized Study Tips
                </Typography>
                <Typography>
                  Based on your recent activity, we recommend focusing on time
                  management and minimizing distractions during study sessions.
                </Typography>
              </Paper>
            </Grid>
          </Grid>
        </Container>
      </Box>
    </Box>
  );
};

export default Dashboard;

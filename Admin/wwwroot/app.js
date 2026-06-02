let apiBaseUrl = "http://localhost:5000";
let users = [];
let groups = [];
let activities = [];

const formatDate = (value) => {
    if (!value) return "-";
    return new Date(value).toLocaleString([], {
        year: "numeric",
        month: "short",
        day: "2-digit",
        hour: "2-digit",
        minute: "2-digit"
    });
};

const apiGet = async (path) => {
    const response = await fetch(`${apiBaseUrl}${path}`, {
        headers: { "Accept": "application/json" }
    });

    if (!response.ok) {
        throw new Error(`${path} returned ${response.status}`);
    }

    return response.json();
};

const setStatus = (message, isError = false) => {
    document.getElementById("statusText").textContent = message;
    document.querySelector(".status-dot").classList.toggle("error", isError);
};

const setStat = (id, value) => {
    document.getElementById(id).textContent = value ?? 0;
};

const renderUsers = () => {
    const search = document.getElementById("userSearch").value.trim().toLowerCase();
    const body = document.getElementById("usersTable");
    const filtered = users.filter(user =>
        `${user.name} ${user.email}`.toLowerCase().includes(search)
    );

    if (filtered.length === 0) {
        body.innerHTML = `<tr><td colspan="4" class="empty">No users found.</td></tr>`;
        return;
    }

    body.innerHTML = filtered.map(user => `
        <tr>
            <td><strong>${user.name ?? "Unknown"}</strong><span>${user.id}</span></td>
            <td>${user.email ?? "-"}</td>
            <td><span class="pill">${user.groupCount ?? 0}</span></td>
            <td>${formatDate(user.createdAt)}</td>
        </tr>
    `).join("");
};

const renderGroups = () => {
    const search = document.getElementById("groupSearch").value.trim().toLowerCase();
    const body = document.getElementById("groupsTable");
    const filtered = groups.filter(group =>
        `${group.name} ${group.description ?? ""} ${group.inviteCode}`.toLowerCase().includes(search)
    );

    if (filtered.length === 0) {
        body.innerHTML = `<tr><td colspan="5" class="empty">No groups found.</td></tr>`;
        return;
    }

    body.innerHTML = filtered.map(group => `
        <tr>
            <td><strong>${group.name ?? "Untitled"}</strong><span>${group.description ?? "No description"}</span></td>
            <td><span class="pill">${group.inviteCode ?? "-"}</span></td>
            <td>${group.createdBy?.name ?? "-"}</td>
            <td>${group.memberCount ?? 0}</td>
            <td>${group.resourceCount ?? 0} resources<br>${group.sessionCount ?? 0} sessions<br>${group.taskCount ?? 0} tasks</td>
        </tr>
    `).join("");
};

const renderActivities = () => {
    const list = document.getElementById("activityList");
    if (activities.length === 0) {
        list.innerHTML = `<div class="empty">No recent activity.</div>`;
        return;
    }

    list.innerHTML = activities.map(item => `
        <div class="activity-item">
            <div>
                <strong>${item.title ?? "Activity"}</strong>
                <span>${item.type ?? "Item"} by ${item.user ?? "Unknown"}</span>
            </div>
            <span>${formatDate(item.date)}</span>
        </div>
    `).join("");
};

const loadDashboard = async () => {
    setStatus("Loading admin data...");
    document.getElementById("refreshBtn").disabled = true;

    try {
        const [dashboard, allUsers, allGroups, recentActivities] = await Promise.all([
            apiGet("/api/admin/dashboard"),
            apiGet("/api/admin/users"),
            apiGet("/api/admin/groups"),
            apiGet("/api/admin/activities")
        ]);

        users = allUsers;
        groups = allGroups;
        activities = recentActivities;

        setStat("totalUsers", dashboard.totalUsers);
        setStat("totalGroups", dashboard.totalGroups);
        setStat("totalResources", dashboard.totalResources);
        setStat("totalSessions", dashboard.totalSessions);
        setStat("totalTasks", dashboard.totalTasks);

        renderUsers();
        renderGroups();
        renderActivities();

        setStatus("Connected to StudyConnect API");
        document.getElementById("lastUpdated").textContent = `Updated ${new Date().toLocaleTimeString()}`;
    } catch (error) {
        setStatus(`API error: ${error.message}`, true);
    } finally {
        document.getElementById("refreshBtn").disabled = false;
    }
};

const init = async () => {
    try {
        const config = await fetch("/config").then(response => response.json());
        apiBaseUrl = config.apiBaseUrl || apiBaseUrl;
    } catch {
        apiBaseUrl = "http://localhost:5000";
    }

    document.getElementById("apiBaseUrl").textContent = apiBaseUrl;
    document.getElementById("refreshBtn").addEventListener("click", loadDashboard);
    document.getElementById("userSearch").addEventListener("input", renderUsers);
    document.getElementById("groupSearch").addEventListener("input", renderGroups);

    await loadDashboard();
};

init();

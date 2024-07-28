//*****************************************************************************
//** 2045. Second Minimum Time to Reach Destination leetcode                 **
//** This was a pain in the butt.  I think I can optimize it for speed. -Dan **
//*****************************************************************************


#define INF INT_MAX

typedef struct {
    int vertex;
    int weight;
} Edge;

typedef struct {
    Edge *edges;
    int size;
} Node;

typedef struct {
    int node;
    int time;
} State;

typedef struct {
    State *states;
    int size;
    int capacity;
} PriorityQueue;

void pq_push(PriorityQueue *pq, int node, int time) {
    if (pq->size == pq->capacity) {
        pq->capacity *= 2;
        pq->states = realloc(pq->states, pq->capacity * sizeof(State));
    }
    pq->states[pq->size].node = node;
    pq->states[pq->size].time = time;
    pq->size++;

    // Bubble up
    int i = pq->size - 1;
    while (i > 0) {
        int parent = (i - 1) / 2;
        if (pq->states[i].time < pq->states[parent].time) {
            State temp = pq->states[i];
            pq->states[i] = pq->states[parent];
            pq->states[parent] = temp;
            i = parent;
        } else {
            break;
        }
    }
}

State pq_pop(PriorityQueue *pq) {
    if (pq->size == 0) {
        State empty = {-1, INF};
        return empty;
    }
    State result = pq->states[0];
    pq->states[0] = pq->states[--pq->size];

    // Bubble down
    int i = 0;
    while (2 * i + 1 < pq->size) {
        int child = 2 * i + 1;
        if (child + 1 < pq->size && pq->states[child + 1].time < pq->states[child].time) {
            child++;
        }
        if (pq->states[i].time > pq->states[child].time) {
            State temp = pq->states[i];
            pq->states[i] = pq->states[child];
            pq->states[child] = temp;
            i = child;
        } else {
            break;
        }
    }
    return result;
}

bool pq_is_empty(PriorityQueue *pq) {
    return pq->size == 0;
}

int secondMinimum(int n, int** edges, int edgesSize, int* edgesColSize, int time, int change) {
    Node *graph = malloc(n * sizeof(Node));
    for (int i = 0; i < n; i++) {
        graph[i].edges = NULL;
        graph[i].size = 0;
    }

    for (int i = 0; i < edgesSize; i++) {
        int u = edges[i][0] - 1;
        int v = edges[i][1] - 1;
        graph[u].edges = realloc(graph[u].edges, (graph[u].size + 1) * sizeof(Edge));
        graph[v].edges = realloc(graph[v].edges, (graph[v].size + 1) * sizeof(Edge));
        graph[u].edges[graph[u].size++] = (Edge){v, time};
        graph[v].edges[graph[v].size++] = (Edge){u, time};
    }

    int *dist = malloc(n * 2 * sizeof(int));
    for (int i = 0; i < n; i++) {
        dist[2 * i] = INF;
        dist[2 * i + 1] = INF;
    }
    dist[0] = 0;

    PriorityQueue pq = {malloc(n * sizeof(State)), 0, n};
    pq_push(&pq, 0, 0);

    while (!pq_is_empty(&pq)) {
        State state = pq_pop(&pq);
        int u = state.node;
        int t = state.time;

        for (int i = 0; i < graph[u].size; i++) {
            Edge edge = graph[u].edges[i];
            int v = edge.vertex;
            int w = edge.weight;

            // Calculate waiting time
            int signal_cycle = 2 * change;
            int current_cycle_position = t % signal_cycle;
            int wait_time = (current_cycle_position >= change) ? signal_cycle - current_cycle_position : 0;

            int next_time = t + w + wait_time;

            if (next_time < dist[2 * v]) {
                dist[2 * v + 1] = dist[2 * v];
                dist[2 * v] = next_time;
                pq_push(&pq, v, next_time);
            } else if (next_time > dist[2 * v] && next_time < dist[2 * v + 1]) {
                dist[2 * v + 1] = next_time;
                pq_push(&pq, v, next_time);
            }
        }
    }

    free(pq.states);
    for (int i = 0; i < n; i++) {
        free(graph[i].edges);
    }
    free(graph);

    return dist[2 * (n - 1) + 1] == INF ? 12 : dist[2 * (n - 1) + 1];
}
